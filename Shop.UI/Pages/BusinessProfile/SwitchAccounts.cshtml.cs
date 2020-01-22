using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile
{
    public class SwitchAccountsModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public SwitchAccountsModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public string Message { get; set; }
        public List<AccountUser> Accounts { get; private set; }

        public void OnGet(string message)
        {
            Message = message;

            Accounts = _ctx.AccountUsers
                .Include(x => x.Account)
                .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();
        }

        public async Task<IActionResult> OnGetUpdateAccount(int accountId)
        {
            Accounts = _ctx.AccountUsers
                .Where(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value)
                .ToList();

            foreach (var acc in Accounts)
            {
                if (acc.AccountId == accountId)
                {
                    acc.Active = true;
                }
                else
                {
                    acc.Active = false;
                }
            }

            await _ctx.SaveChangesAsync();

            return RedirectToPage("/BusinessProfile/SwitchAccounts", new { message = "Accounts Switched" });
        }
    }
}