using Microsoft.AspNetCore.Mvc;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models.EmailTemplates;
using Shop.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.UI.Controllers
{

    public class EmailController : BaseController
    {
        private EmailSender _emailSender;
        public EmailController(EmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public IActionResult Email() =>
            Ok(_emailSender.SendTransactionalEmail(GetUserEmail()));

        public async Task<IActionResult> SendProductEmail(string email, string productId, string message, [FromServices] ApplicationDbContext _context)
        {
            //todo: configure url, using staging for now
            //var callbackUrl = 
            //    $"http://officebox.eu-west-2.elasticbeanstalk.com/Shop/Product/{productId}";

            var me = await _context.Users.FindAsync(GetUserId());
            string callbackUrl = Url.Action("Product", "Shop", new { id = productId }, Request.Scheme);
            var product = _context.Products.Find(productId);

            string image = string.Empty;
            if (!string.IsNullOrEmpty(product.ImageUrl))
                image = string.Format("<img style='width: 100px;' src='{0}' />", product.ImageUrl);

            Dictionary<string, string> vars = new Dictionary<string, string> { };
            vars.Add("sendername", string.Format("{0} {1}", me?.FirstName, me?.LastName));
            vars.Add("type", "product share");
            vars.Add("sendermessage", string.Format("<a href='{0}'>{1} {2}</a>", callbackUrl, image, product.Name));

            //await _emailSender.SendProductEmail(email, callbackUrl);
            Message m = new Message
            {
                FromEmail = "orders@officebox.co.za",
                FromName = "Officebox",
                Subject = "Product Share",
                To = new To[]
                {
                      new To {  Email = email}
                }
            };

            _emailSender.SendEmailByTemplate(vars, "mail-quotation", m);
            return RedirectToPage("/Shop/Index");
        }

    }
}