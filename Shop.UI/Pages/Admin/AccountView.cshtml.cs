using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.UI.Infrastructure;

namespace Shop.UI.Pages.Admin
{
    public class AccountViewModel : BasePage
    {
        public ApplicationDbContext _ctx;

        public AccountViewModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [BindProperty]
        public Domain.Models.Accounts.Account Account { get; set; }

        public void OnGet(int id)
        {
            Account = _ctx.Accounts
                        .Include(a => a.Address)
                        .Include(a => a.AccountUsers)
                        .Include(a => a.Locations)
                        .Include(a => a.References)
                        .FirstOrDefault(a => a.Id == id);

            foreach (var user in Account.AccountUsers)
            {
                user.User = _ctx.Users.Find(user.UserId);
            }

            foreach (var location in Account.Locations)
            {
                location.Address = _ctx.Addresses.Find(location.AddressId);

                location.LocationAuths = _ctx.LocationAuth.Where(l => l.LocationId == location.Id).ToList();
                foreach (var locationAuth in location.LocationAuths)
                {
                    locationAuth.User = _ctx.Users.Find(locationAuth.Userid);
                }

                location.CostCenters = _ctx.CostCenters.Where(c => c.LocationId == location.Id).ToList();
                foreach (var costCenter in location.CostCenters)
                {
                    costCenter.CostCenterAuth = _ctx.CostCenterAuth.Where(c => c.CostCenterId == costCenter.Id).ToList();
                    foreach(var costCenterAuth in costCenter.CostCenterAuth)
                    {
                        costCenter.User = _ctx.Users.Find(costCenterAuth.Userid);
                    }
                }
            }
        }

        public IActionResult OnPost()
        {
            var acc = _ctx.Accounts.Find(Account.Id);

            acc.CompanyName = Account.CompanyName;
            acc.RegistrationNumber = Account.RegistrationNumber;
            acc.VatNumber = Account.VatNumber;
            acc.PhoneNumber = Account.PhoneNumber;
            acc.Email = Account.Email;
            acc.TermAccount = Account.TermAccount;
            acc.ThirtyDayTermApproved = Account.ThirtyDayTermApproved;
            acc.Limit = acc.Limit;

            _ctx.SaveChanges();

            return RedirectToPage("/Admin/AccountView", new { id = Account.Id });
        }
    }
}