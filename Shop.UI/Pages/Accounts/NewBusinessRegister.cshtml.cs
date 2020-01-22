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
    public class NewBusinessRegisterModel : BasePage
    {
        private IConfiguration _config;
        private EmailSender _emailSender;

        public NewBusinessRegisterModel(IConfiguration config, EmailSender emailSender)
        {
            _config = config;
            _emailSender = emailSender;
        }

        [BindProperty]
        public NewBusinessRegisterViewModel Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(
             [FromServices] ApplicationDbContext ctx,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] SignInManager<ApplicationUser> signInManager
            )
        {

            if (!Input.RequestPayment)
            {
                ModelState.ClearValidationState("Input.PayerName");
                ModelState.ClearValidationState("Input.PayerEmail");
                ModelState.MarkFieldValid("Input.PayerName");
                ModelState.MarkFieldValid("Input.PayerEmail");
            }

            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email
            };

            var address = new Address
            {
                Address1 = Input.Address1,
                Address2 = Input.Address2,
                City = Input.City,
                PostCode = Input.PostCode,
            };

            user.Addresses.Add(address);

            var result = await userManager.CreateAsync(user, Input.Password);

            if (!result.Succeeded)
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
                          });            }
            return Page();
        }

        public class NewBusinessRegisterViewModel
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
}