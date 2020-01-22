using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Application.Services.Emails;
using Shop.Domain.Models.EmailTemplates;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.UI.Pages.ContactPromotions
{
    public class IndexModel : PageModel
    {
        private EmailSender _emailSender;
        public IndexModel(EmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [BindProperty]
        public ContactForm Input { get; set; }

        public async Task<IActionResult> OnPost()
        {
            Dictionary<string, string> vars = new Dictionary<string, string> { };
            vars.Add("contactname", Input.Name);
            vars.Add("representing", Input.Representing);
            vars.Add("contactemail", Input.Email);
            vars.Add("contactnumber", Input.Number);
            vars.Add("contactmessage", Input.Message);

            Message m = new Message
            {
                FromEmail = "orders@officebox.co.za",
                FromName = "Officebox",
                Subject = "OfficeBox support ticket",
                To = new To[]
                  {
                      new To {  Email = "customerservice@officebox.co.za", Name = "OfficeBox"}
                      //new To {  Email = "a.carel.g.nel@gmail.com", Name = "OfficeBox"}
                  }
            };

            _emailSender.SendEmailByTemplate(vars, "promotionsContactUs", m);
            //await _emailSender.sendSupportEmail("customerservice@officebox.co.za", vars, "contact-us", m);

            return RedirectToPage("/ContactPromotions/Confirmation");
        }

        public class ContactForm
        {
            public string Name { get; set; }
            public string Representing { get; set; }
            public string Email { get; set; }
            public string Number { get; set; }
            public string Message { get; set; }
        }
    }
}