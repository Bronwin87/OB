using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using System.Linq;
using System.Security.Claims;

namespace Shop.UI.Pages.BusinessProfile.CompanyDetails
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public IndexModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public string Message { get; set; }
        public Account Account { get; set; }

        public void OnGet(string message)
        {
            Message = message;

            var user = _ctx.AccountUsers.FirstOrDefault(au => au.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var account = _ctx.Accounts.FirstOrDefault(ac => ac.Id == user.AccountId);

            Account = _ctx.AccountUsers
                .Include(x => x.Account)
                .ThenInclude(x => x.Address)
                .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                .Account;
        }
    }
}