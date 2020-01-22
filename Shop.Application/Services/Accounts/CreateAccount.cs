using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.EmailTemplates;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shop.Application.Accounts
{
    public class CreateAccount
    {
        private ApplicationDbContext _ctx;
        private EmailSender _emailSender;

        public CreateAccount(ApplicationDbContext ctx, EmailSender emailSender)
        {
            _ctx = ctx;
            _emailSender = emailSender;
        }

        public class Request
        {
            public string UserId { get; set; }
            public BusinessRegisterViewModel Input { get; set; }
        }

        public class BusinessRegisterViewModel
        {
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string LastName { get; set; }
            [Required]
            [DataType(DataType.EmailAddress)]
            public string Email { get; set; }
            [Required]
            public string PhoneNumber { get; set; }
            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
            [Required]
            [DataType(DataType.Password)]
            [Compare("Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string CompanyName { get; set; }
            //public string ContractNumber { get; set; }
            [Required]
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string Country { get; set; } = "SOUTH AFRICA";
            [Required]
            public string PostCode { get; set; }


            [Required]
            public double? CreditLimit { get; set; }
            [Required]
            public string RegistrationNumber { get; set; }
            public string CompanyVAT { get; set; }

            public List<ReferenceViewModel> References { get; set; }

            public bool TermAccount { get; set; }

            public string TradingName { get; set; }
            public string AvgMonthly { get; set; }
            public bool AgreeTOC { get; set; }
        }

        public class ReferenceViewModel
        {
            [Required]
            public string CompanyName { get; set; }
            [Required]
            public string FirstName { get; set; }
            [Required]
            public string SureName { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public string Telephone { get; set; }
        }

        public async Task<int> Do(Request request)
        {
            var Account = new Account
            {
                CompanyName = request.Input.CompanyName,
                RegistrationNumber = request.Input.RegistrationNumber,
                VatNumber = request.Input.CompanyVAT,
                TermAccount = request.Input.TermAccount,
                Limit = request.Input.CreditLimit ?? 0,
                ThirtyDayTermApproved = false,
            };

            var AccountAddress = new Domain.Models.Address
            {
                Address1 = request.Input.Address1,
                Address2 = request.Input.Address2,
                City = request.Input.City,
                PostCode = request.Input.PostCode,
            };

            Account.Address = AccountAddress;

            Account.Locations.Add(new Location
            {
                Name = request.Input.CompanyName,
                UserId = request.UserId,
                Address = AccountAddress
            });

            StringBuilder tradeReferenceBuilder = new StringBuilder();
            string thTemplate = "<td valign='top' style='font-size:12px;font-family:Arial, Helvetica, sans-serif;border-bottom:1px solid #e9e9e9;border-right:1px solid #e9e9e9;'>";
            string tdTemplate = "<td valign='top' style='font-size:12px;font-family:Arial, Helvetica, sans-serif;border-bottom:1px solid #e9e9e9;background-color:#ffffff;'>";
            tradeReferenceBuilder.AppendFormat("<table border='0' width='100%' cellspacing='0' cellpadding='15' style='background-color:#fbfbfb;border:1px solid #e9e9e9;'><tr>{0}Company Name</th>{0}Contact Name</th>{0}Email</th>{0}Telephone</th></tr>", thTemplate);
            if (request.Input.TermAccount)
            {
                foreach (var r in request.Input.References)
                {
                    Account.References.Add(new Reference
                    {
                        CompanyName = r.CompanyName,
                        Email = r.Email,
                        ContactName = r.FirstName + " " + r.SureName,
                        Telephone = r.Telephone
                    });

                    tradeReferenceBuilder.AppendFormat("<tr>{0}{1}</td>{0}{2}</td>{0}{3}</td>{0}{4}</td></tr>",
                        tdTemplate,
                        r.CompanyName,
                        string.Format("{0} {1}", r.FirstName, r.SureName),
                        r.Email,
                        r.Telephone);
                }
            }
            tradeReferenceBuilder.Append("</table>");

            var User = _ctx.Users
                .Include(x => x.Accounts)
                .FirstOrDefault(x => x.Id == request.UserId);

            foreach (var acc in User.Accounts)
            {
                acc.Active = false;
            }

            Account.AccountUsers.Add(new AccountUser
            {
                UserId = User.Id,
                Active = true
            });

            _ctx.Accounts.Add(Account);

            await _ctx.SaveChangesAsync();

            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("contactname", request.Input.FirstName);
            vars.Add("contactemail", request.Input.Email);
            vars.Add("contactphone", request.Input.PhoneNumber);
            vars.Add("companyname", request.Input.CompanyName);
            vars.Add("companyaddress", request.Input.Address1 + " , " + request.Input.Address2 + " , " + request.Input.City + " , " + request.Input.Country);
            vars.Add("30dayterms", request.Input.TermAccount ? "Yes" : "No");
            vars.Add("creditlimit", request.Input.CreditLimit?.ToString());
            vars.Add("companyregistrationnumber", request.Input.RegistrationNumber);
            vars.Add("companyvatnumber", request.Input.CompanyVAT);
            vars.Add("tradereferences", tradeReferenceBuilder.ToString());

            Message m = new Message
            {
                FromEmail = "orders@officebox.co.za",
                FromName = "Officebox",
                Subject = "OfficeBox Business Registration - " + request.Input.CompanyName,
                To = new To[]
                {
                    new To {  Email = "david@officebox.co.za", Name = "David Adams"}
                }
            };

            //send new business registration email to david
            _emailSender.SendEmailByTemplate(vars, "BusinessRegistration", m);

            return Account.Id;
        }
    }
}
