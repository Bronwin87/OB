using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shop.Application.Oders;
using Shop.Application.Payment;
using Shop.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Accounts
{
    public class GuestBusinessTermProfileModel : BasePage
    {

        private IConfiguration _config;

        public GuestBusinessTermProfileModel(IConfiguration config)
        {
            _config = config;
        }

        public void OnGet()
        {
        }

        [BindProperty]
        public GuestBusinessRequestPaymentViewModel Input { get; set; }

        public async Task<IActionResult> OnPostAsync(
            [FromBody] IEnumerable<GuestBusinessProfileRegisterViewModel> model)
        {
            if (!ModelState.IsValid)
                return Page();
            //todo handle stuff here 
            return new OkResult();
        }

        public async Task<IActionResult> OnPostRequestPayment(
            [FromServices] OneOffPayment payment,
            [FromServices] CreateOrder createOrder)
        {
            var PaymentReference = Guid.NewGuid().ToString();

            var order = await createOrder.Do(new CreateOrder.Request
            {
                UserMark = GetCartUserMark(),
                PayementReference = PaymentReference,
            });

            var totalValue = order.OrderProducts.Sum(x => x.Product.Price * x.Qty);

            var redirectUrl = payment.GetRedirectLink(new OneOffPayment.Request
            { 
                BuyerEmail = GetUserEmail(),
                PaymentReference = PaymentReference,
                TotalValue = totalValue ?? 0m
            });

            var client = new SmtpClient(_config["Email:SMTP"], Convert.ToInt32(_config["Email:PORT"]))
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"])
            };

            var mailMessage = new MailMessage
            {
                To = { Input.PayerEmail },
                Subject = "Order Subject",
                Body = $@"
<h1>Order Approval</h1>


<a href=""{redirectUrl}"">Approve Order Here</a>
",
                From = new MailAddress(_config["Email:Username"]),
                IsBodyHtml = true
            };

            await client.SendMailAsync(mailMessage);
            return RedirectToPage("/Checkout/Confirmation");
        }
    }

    public class GuestBusinessProfileRegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }

        public bool Authorizer { get; set; }
    }

    public class GuestBusinessRequestPaymentViewModel
    {
        public string PayerName { get; set; }
        public string PayerEmail { get; set; }
    }

}