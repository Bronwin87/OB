using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Shop.Application.Cart;
using Shop.Application.Oders;
using Shop.Application.Payment;
using Shop.Domain.Models;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Threading.Tasks;
using Shop.Application.Accounts;
using Shop.Database;
using Shop.Application.Services.Emails;

namespace Shop.UI.Pages.Accounts
{
    public class GuestBusinessRegisterModel : BasePage
    {
        private IConfiguration _config;
        private EmailSender _emailSender;

        public GuestBusinessRegisterModel(IConfiguration config, EmailSender emailSender)
        {
            _config = config;
            _emailSender = emailSender;
        }

        [BindProperty]
        public GuestBusinessRegisterViewModel Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(
             [FromServices] ApplicationDbContext ctx,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] SignInManager<ApplicationUser> signInManager,
            [FromServices] OneOffPayment payment,
            [FromServices] SetCartAddress setCartAddress,
            [FromServices] CreateOrder createOrder)
        {
            if (!Input.RequestPayment)
            {
                ModelState.ClearValidationState("Input.PayerName");
                ModelState.ClearValidationState("Input.PayerEmail");
                ModelState.MarkFieldValid("Input.PayerName");
                ModelState.MarkFieldValid("Input.PayerEmail");
            }

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email
            };

            foreach (var item in userManager.PasswordValidators)
            {
                var password = await item.ValidateAsync(userManager, user, Input.Password);
                if (!password.Succeeded)
                {
                    foreach (var error in password.Errors)
                    {
                        ModelState.AddModelError("Input.Password", string.Format("[{0}] - {1}", error.Code, error.Description));
                    }
                }
            }

            if (!ModelState.IsValid)
                return Page();

            var address = new Address
            {
                Address1 = Input.Address1,
                Address2 = Input.Address2,
                City = Input.City,
                PostCode = Input.PostCode,
            };

            user.Addresses.Add(address);

            var result = await userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim("type", "superuser"));

                await signInManager.SignInAsync(user, false);
                await new CreateAccount(ctx, _emailSender)
                           .Do(new CreateAccount.Request
                           {
                               UserId = user.Id,
                               Input = new CreateAccount.BusinessRegisterViewModel
                               {
                                   CompanyName = Input.CompanyName,
                                   RegistrationNumber = "",
                                   CompanyVAT = "",
                                   TermAccount = false,
                                   Address1 = Input.Address1,
                                   Address2 = Input.Address2,
                                   City = Input.City,
                                   PostCode = Input.PostCode,
                               }
                           });

                await setCartAddress.Do(new SetCartAddress.Request
                {
                    UserMark = GetCartUserMark(),
                    AddressId = address.Id
                });

                if (Input.RequestPayment)
                {
                    var PaymentReference = Guid.NewGuid().ToString();

                    var order = await createOrder.Do(new CreateOrder.Request
                    {
                        UserMark = (user.Id, GetSessionId()),
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
                else
                {
                    return RedirectToPage("/Checkout/Payment");
                }
            }
            return Page();
        }
    }

    public class GuestBusinessRegisterViewModel
    {
        [Required]
        public string CompanyName { get; set; }
        public string CompanyVat { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public bool RequestPayment { get; set; }

        [Required]
        public string PayerName { get; set; }
        [Required]
        public string PayerEmail { get; set; }

        [Required]
        public string LocationType { get; set; }
        public string Company { get; set; }
        [Required]
        public string RecieversFirstName { get; set; }
        [Required]
        public string RecieversLastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostCode { get; set; }
    }
}