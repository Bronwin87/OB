using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using Shop.UI.Pages.Shared;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Admin
{
    public class AccountManagerModel : PageModel
    {
        const int PAGE_SIZE = 25;
        private ApplicationDbContext _ctx;

        public PagerSourceList<Account> Accounts { get; set; }

        public AccountManagerModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task OnGetAsync(int? pageIndex)
        {
            Accounts = await PagerSourceList<Account>.CreateAsync(_ctx.Accounts, pageIndex ?? 1, PAGE_SIZE);
        }

        public async Task<IActionResult> OnGetApprove(int id)
        {
            var account = _ctx.Accounts
                .Include(x => x.Address)
                .FirstOrDefault(x => x.Id == id);
            account.ThirtyDayTermApproved = true;
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/Admin/AccountManager");
        }

        public async Task<IActionResult> OnGetDisable(int id)
        {
            var account = _ctx.Accounts.FirstOrDefault(x => x.Id == id);
            account.ThirtyDayTermApproved = false;
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/Admin/AccountManager");
        }
    }
}