using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PayFast;
using PayFast.AspNetCore;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.EmailTemplates;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Shop.UI.Controllers
{
    [Route("api/[controller]")]
    public class PaymentHookController : Controller
    {
        private readonly ILogger logger;

        private readonly PayFastSettings _payFastSettings;
        private readonly ApplicationDbContext _ctx;
        private readonly EmailSender _emailSender;

        public PaymentHookController(IOptions<PayFastSettings> payFastSettings, ApplicationDbContext ctx, EmailSender emailSender, ILogger<PaymentHookController> logger)
        {
            this._payFastSettings = payFastSettings.Value;
            _ctx = ctx;
            this._emailSender = emailSender;
            this.logger = logger;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<controller>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        //public void Post([FromBody]string value)
        public async Task<IActionResult> Post([ModelBinder(BinderType = typeof(PayFastNotifyModelBinder))]PayFastNotify payFastNotifyViewModel)
        {
            payFastNotifyViewModel.SetPassPhrase(this._payFastSettings.PassPhrase);
            var calculatedSignature = payFastNotifyViewModel.GetCalculatedSignature();
            var isValid = payFastNotifyViewModel.signature == calculatedSignature;

            // The PayFast Validator is still under developement
            // Its not recommended to rely on this for production use cases
            var payfastValidator = new PayFastValidator(this._payFastSettings, payFastNotifyViewModel, this.HttpContext.Connection.RemoteIpAddress);

            var merchantIdValidationResult = payfastValidator.ValidateMerchantId();

            var ipAddressValidationResult = await payfastValidator.ValidateSourceIp();

            // Currently seems that the data validation only works for success
            if (payFastNotifyViewModel.payment_status == PayFastStatics.CompletePaymentConfirmation)
            {
                var dataValidationResult = await payfastValidator.ValidateData();

                //Complete Order
                //Todo take this to the application layer service
                //var order = await _ctx.Orders.FirstOrDefaultAsync(x => x.PaymentReference == payFastNotifyViewModel.m_payment_id);
                var order = await _ctx.Orders
                                      .Include(x => x.User)
                                      .FirstOrDefaultAsync(x => x.OrderNumber == payFastNotifyViewModel.m_payment_id);

                //Place the order
                order.Status = Domain.Enums.OrderStatus.Placed;

                //do cin7 push here
                //var so = await Cin7SalesOrderRequest(order);
                await _ctx.SaveChangesAsync();

                string cmsLink = Url.Action("SingleOrder", "Admin", new { id = order.OrderNumber }, Request.Scheme);


                var address = await _ctx.Addresses.FirstOrDefaultAsync(a => a.Id == order.AddressId);

                Console.WriteLine("PAYFAST LOG TEST");

                if (order.Account != null)
                {
                    Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    //    { "location-costcenter", string.Format("", order.Account.CompanyName, order.Account.)},
                    { "orderaddress", string.Format("{0}<br/>{1}<br/>{2}<br/>{3}<br/>{4}<br/>{5}", order.Account.CompanyName,order.Location.Name,address?.Address1, address?.Address2, address?.City, address?.PostCode) },
                    { "cmslink", string.Format("<a href='{0}'>{0}</a>", cmsLink) }
                };

                    Message m = new Message
                    {
                        FromEmail = "orders@officebox.co.za",
                        FromName = "Officebox",
                        Subject = string.Format("OfficeBox Order Placed - {0}", order.OrderNumber),
                        To = new To[]
                   {
                        new To { Email = "orders@officebox.co.za", Name = "OfficeBox" }
                        //new To { Email = "a.carel.g.nel@gmail.com", Name = "Carel" }
                   }
                    };

                    _emailSender.SendEmailByTemplate(vars, "internal-order-confirmation-account", m);
                }
                else
                {
                    Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal) },
                    { "orderdate", DateTime.Now.ToString("dd MMM yyyy") },
                    { "orderaddress", string.Format("{0}<br/>{1}<br/>{2}<br/>{3}", address?.Address1, address?.Address2, address?.City, address?.PostCode) },
                    { "cmslink", string.Format("<a href='{0}'>{0}</a>", cmsLink) }
                };

                    Message m = new Message
                    {
                        FromEmail = "orders@officebox.co.za",
                        FromName = "Officebox",
                        Subject = string.Format("OfficeBox Order Placed - {0}", order.OrderNumber),
                        To = new To[]
                   {
                        new To { Email = "orders@officebox.co.za", Name = "OfficeBox" }
                        //new To { Email = "a.carel.g.nel@gmail.com", Name = "Carel" }
                   }
                    };

                    _emailSender.SendEmailByTemplate(vars, "internal-order-confirmation", m);
                }
               

               
                //_emailSender.SendEmailByTemplate("a.carel.g.nel@gmail.com", vars, "internal-order-confirmation", m);

                Dictionary<string, string> vars2 = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", order.User.FirstName) },
                    { "greetingdescription", string.Format("Your order {0} has been confirmed and is being processed accordingly.", order.OrderNumber) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    { "ordertime", DateTime.UtcNow.ToString("hh:mm tt") },
                    { "orderaddress", string.Format("{0}<br/>{1}<br/>{2}<br/>{3}", address?.Address1, address?.Address2, address?.City, address?.PostCode) },
                };

                Message m2 = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Awesome - We’ve received the order - {0}", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = order.User.Email, Name = order.User.FirstName }
                    }
                };

                _emailSender.SendEmailByTemplate(vars2, "finalcomfirmation", m2);
            }
            else if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                //Cancel Order - delete it 
                //Todo take this to the application layer service
                var order = _ctx.Orders.FirstOrDefault(x => x.PaymentReference == payFastNotifyViewModel.m_payment_id);
                _ctx.Orders.Remove(order);
                await _ctx.SaveChangesAsync();
            }

            return Ok();
        }
    }
}
