using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using PayFast;
using PayFast.AspNetCore;
using Shop.Database;
using System;
using System.Linq;
using System.Threading.Tasks;
using Shop.Domain.Models;
using Cin7ApiWrapper.Models;
using Cin7ApiWrapper;
using Cin7ApiWrapper.Common;
using Cin7ApiWrapper.Infrastructure;
using System.Collections.Generic;
using Shop.Domain.Models.Accounts;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.EmailTemplates;
using Shop.Application.Services.Emails;
using Microsoft.Extensions.Logging;

namespace Shop.UI.Controllers.Payfast
{
    [Route("[controller]")]
    public class PayFastHookController : Controller
    {
        private readonly ILogger logger;

        private readonly PayFastSettings _payFastSettings;
        private readonly ApplicationDbContext _ctx;
        private readonly EmailSender _emailSender;

        public PayFastHookController(IOptions<PayFastSettings> payFastSettings, ApplicationDbContext ctx, EmailSender emailSender, ILogger<PayFastHookController> logger)
        {
            this._payFastSettings = payFastSettings.Value;
            _ctx = ctx;
            this._emailSender = emailSender;
            this.logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> NotifyTest()
        {
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Notify([ModelBinder(BinderType = typeof(PayFastNotifyModelBinder))]PayFastNotify payFastNotifyViewModel)
        {
            logger.LogInformation("Payfast Notify URL accessed");

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

                //Compelte Order
                //Todo take this to the application layer service
                var order = _ctx.Orders.FirstOrDefault(x => x.PaymentReference == payFastNotifyViewModel.m_payment_id);

                //Place the order
                order.Status = Domain.Enums.OrderStatus.Placed;

                //do cin7 push here
                //var so = await Cin7SalesOrderRequest(order);
                await _ctx.SaveChangesAsync();

                string cmsLink = Url.Action("SingleOrder", "Admin", new { id = order.OrderNumber }, Request.Scheme);

                Location location = await _ctx.Locations
                                         .Include(l => l.Address)
                                         .Include(l => l.Authorizer)
                                         .FirstOrDefaultAsync(l => l.Id == order.LocationId);

                Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", location.Name, location.Address.Address1, location.Address.Address2, location.Address.City, location.Address.PostCode) },
                    { "cmslink", string.Format("<a href='{0}'>{0}</a>", cmsLink) },
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
                //_emailSender.SendEmailByTemplate("a.carel.g.nel@gmail.com", vars, "internal-order-confirmation", m);
            }
            else if (payFastNotifyViewModel.payment_status == PayFastStatics.CancelledPaymentConfirmation)
            {
                //Cancle Order - delete it 
                //Todo take this to the application layer service
                var order = _ctx.Orders.FirstOrDefault(x => x.PaymentReference == payFastNotifyViewModel.m_payment_id);

                _ctx.Orders.Remove(order);

                string cmsLink = Url.Action("SingleOrder", "Admin", new { id = order.OrderNumber }, Request.Scheme);

                Location location = await _ctx.Locations
                                         .Include(l => l.Address)
                                         .Include(l => l.Authorizer)
                                         .FirstOrDefaultAsync(l => l.Id == order.LocationId);

                Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", location.Name, location.Address.Address1, location.Address.Address2, location.Address.City, location.Address.PostCode) },
                    { "cmslink", string.Format("<a href='{0}'>OfficeBox Order Cancelled</a>", cmsLink) },
                };

                Message m = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("OfficeBox Order Cancelled - {0}", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = "orders@officebox.co.za", Name = "OfficeBox" }
                        //new To { Email = "a.carel.g.nel@gmail.com", Name = "Carel" }
                    }
                };

                _emailSender.SendEmailByTemplate(vars, "internal-order-confirmation", m);
                //_emailSender.SendEmailByTemplate("a.carel.g.nel@gmail.com", vars, "internal-order-confirmation", m);

                await _ctx.SaveChangesAsync();
            }

            return Ok();
        }

        public IActionResult Recurring()
        {
            var recurringRequest = new PayFastRequest(this._payFastSettings.PassPhrase);

            //Merchan Details
            recurringRequest.merchant_id = this._payFastSettings.MerchantId;
            recurringRequest.merchant_key = this._payFastSettings.MerchantKey;
            recurringRequest.return_url = this._payFastSettings.ReturnUrl;
            recurringRequest.cancel_url = this._payFastSettings.CancelUrl;
            recurringRequest.notify_url = this._payFastSettings.NotifyUrl;

            // Buyer Details
            recurringRequest.email_address = User.Identity.Name;

            // Transaction Details to add officebox details later, this is for testing
            recurringRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            recurringRequest.amount = 20;
            recurringRequest.item_name = "Recurring Option";
            recurringRequest.item_description = "Some details about the recurring option";

            // Transaction Options
            recurringRequest.email_confirmation = true;
            recurringRequest.confirmation_address = "bronwinbergstedt@gmail.com";

            // Recurring Billing Details
            recurringRequest.subscription_type = SubscriptionType.Subscription;
            recurringRequest.billing_date = DateTime.UtcNow;
            recurringRequest.recurring_amount = 20;
            recurringRequest.frequency = BillingFrequency.Monthly;
            recurringRequest.cycles = 0;

            var redirectUrl = $"{this._payFastSettings.ProcessUrl}{recurringRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public IActionResult OnceOff()
        {
            var onceOffRequest = new PayFastRequest(this._payFastSettings.PassPhrase);

            // Merchant Details
            onceOffRequest.merchant_id = this._payFastSettings.MerchantId;
            onceOffRequest.merchant_key = this._payFastSettings.MerchantKey;
            onceOffRequest.return_url = this._payFastSettings.ReturnUrl;
            onceOffRequest.cancel_url = this._payFastSettings.CancelUrl;
            onceOffRequest.notify_url = this._payFastSettings.NotifyUrl;

            // Buyer Details
            onceOffRequest.email_address = User.Identity.Name;

            // Transaction Details, need to change to real details
            onceOffRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            onceOffRequest.amount = 30;
            onceOffRequest.item_name = "Once off option";
            onceOffRequest.item_description = "Some details about the once off payment";
            // Transaction Options
            onceOffRequest.email_confirmation = true;
            onceOffRequest.confirmation_address = "bronwinbergstedt@gmail.com";

            var redirectUrl = $"{this._payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        public IActionResult AdHoc()
        {
            var adHocRequest = new PayFastRequest(this._payFastSettings.PassPhrase);
            // Merchant Details
            adHocRequest.merchant_id = this._payFastSettings.MerchantId;
            adHocRequest.merchant_key = this._payFastSettings.MerchantKey;
            adHocRequest.return_url = this._payFastSettings.ReturnUrl;
            adHocRequest.cancel_url = this._payFastSettings.CancelUrl;
            adHocRequest.notify_url = this._payFastSettings.NotifyUrl;

            // Buyer Details
            adHocRequest.email_address = User.Identity.Name;

            // Transaction Details
            adHocRequest.m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380";
            adHocRequest.amount = 70;
            adHocRequest.item_name = "Adhoc Agreement";
            adHocRequest.item_description = "Some details about the adhoc agreement";

            // Transaction Options
            adHocRequest.email_confirmation = true;
            adHocRequest.confirmation_address = "bronwinbergstedt@gmail.com";

            // Recurring Billing Details
            adHocRequest.subscription_type = SubscriptionType.AdHoc;

            var redirectUrl = $"{this._payFastSettings.ProcessUrl}{adHocRequest.ToString()}";

            return Redirect(redirectUrl);
        }

        [HttpGet]
        public IActionResult Return()
        {
            return View();
        }

        public IActionResult Cancel()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<CreateResult> Cin7SalesOrderRequest(Domain.Models.Orders.Order order)
        {
            var api = new Cin7Api(new ApiUser("StationeryinMotionSA", "1251ada5191046238405f5a66ebbac8f"));
            var createdDate = order.Created;

            var sale = new Cin7ApiWrapper.Models.SalesOrder()
            {
                CreatedDate = order.Created,
                ModifiedDate = DateTime.UtcNow,
                CurrencyCode = "ZAR",
                CurrencySymbol = "R",
                TaxStatus = TaxStatus.Excl,
                TaxRate = 0.14m,
                Total = order.OrderProducts.Sum(x => x.Qty * x.Product.Price) ?? 0,
                FirstName = order.User.FirstName,
                LastName = order.User.LastName,
                Company = order.Account.CompanyName,
                Email = order.User.Email,
                Phone = order.User.PhoneNumber,
                CustomerOrderNo = order.OrderNumber,
                IsApproved = (order.Status == Domain.Enums.OrderStatus.Complete) ? true : false,
                MemberEmail = order.User.Email,
                DeliveryAddress1 = order.Address.Address1,
                DeliveryAddress2 = order.Address.Address2,
                DeliveryCity = order.Address.City,
                DeliveryPostalCode = order.Address.PostCode,
                //MemberCostCenter = order.Address.
                ProductTotal = order.OrderProducts.Sum(x => x.Qty * x.Product.Price) ?? 0,
                //LineItems = order.OrderProducts. add products to this array 
                LineItems = getLineItems(order).ToArray(),
                InternalComments = "DO NOT PROCESS, NEW SITE TESTS"
            };

            CreateResult result = api.SalesOrders.Create(sale);

            return result;
        }

        public List<SalesOrderLineitem> getLineItems(Domain.Models.Orders.Order order)
        {
            List<SalesOrderLineitem> li = new List<SalesOrderLineitem>();
            foreach (var x in order.OrderProducts)
            {
                SalesOrderLineitem soli = new SalesOrderLineitem
                {
                    Id = 1,
                    CreatedDate = DateTime.UtcNow,
                    TransactionId = 3,
                    ProductId = 1,
                    ProductOptionId = 6,
                    StyleCode = x.Product.Colour,
                    Code = x.Product.ExternalId,
                    Name = x.Product.Name,
                    Option1 = x.Product.Unit,
                    Option2 = x.Product.Brand,
                    SizeCodes = "none",
                    LineComments = "none",
                    IntegrationRef = x.Product.Id,
                    Qty = x.Qty,
                    UnitPrice = x.Product.Price ?? 0,
                    Discount = 0,
                    QtyShipped = 0
                };
                li.Add(soli);
            }
            return li;
        }
    }
}