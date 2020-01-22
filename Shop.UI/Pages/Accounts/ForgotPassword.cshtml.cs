using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Shop.Domain.Models.Users;
using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Shop.Domain.Models.EmailTemplates;
using Shop.Application.Services.Emails;
using System.Collections.Generic;

namespace Shop.UI.Pages.Accounts
{
    public class ForgotPasswordModel : PageModel
    {
        private IConfiguration _config;
        private UserManager<ApplicationUser> _userManager;
        private EmailSender _emailSender;

        public ForgotPasswordModel(IConfiguration config, UserManager<ApplicationUser> userManger, EmailSender emailSender)
        {
            _config = config;
            _userManager = userManger;
            _emailSender = emailSender;
        }

        [BindProperty]
        public ForgotPasswordViewModel Input { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Email);

            if (user == null)
                return RedirectToPage("ForgotPasswordConfirmation");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);

            var callbackUrl = Url.Page("/Accounts/ResetPassword", null, new { userId = user.Id, code }, Request.Scheme);

            Dictionary<string, string> vars = new Dictionary<string, string> { };
            vars.Add("link", callbackUrl);
            vars.Add("link-tag", string.Format("<a href={0}>click here</a>", callbackUrl));

            Message m = new Message
            {
                FromEmail = "orders@officebox.co.za",
                FromName = "Officebox",
                Subject = "Password Reset Request",
                To = new To[]
                {
                    new To { Email = Input.Email }
                }
            };

            _emailSender.SendEmailByTemplate(vars, "forgot-password2", m);
            //await _emailSender.sendForgotPasswordEmail("bronwinbergstedt@gmail.com",vars, "forgot-password2", m);

            return RedirectToPage("ForgotPasswordConfirmation");
        }
    }

    public class ForgotPasswordViewModel
    {
        [Required]
        public string Email { get; set; }
    }
}