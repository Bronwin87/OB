using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Users;
using Shop.UI.Pages.Shared;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Shop.UI.Pages.Admin
{
    public class UsersModel : PageModel
    {
        const int PAGE_SIZE = 25;
        private ApplicationDbContext _ctx;

        public PagerSourceList<AccountUser> AccountUsers;

        public UsersModel(ApplicationDbContext ctx)
        {
            this._ctx = ctx;
        }

        public async Task OnGetAsync(int? pageIndex)
        {
            AccountUsers = await PagerSourceList<AccountUser>.CreateAsync(
                _ctx.AccountUsers
                    .Include(au => au.Account)
                    .Include(au => au.User), pageIndex ?? 1, PAGE_SIZE);
        }

        public string GetAccountType(ApplicationUser user)
        {
            switch (user.Type)
            {
                case Domain.Enums.UserType.Authorizer:
                    return "Authorizer";

                case Domain.Enums.UserType.BusinessUser:
                    return "Business";

                case Domain.Enums.UserType.IndividualUser:
                    return "Individual";

                case Domain.Enums.UserType.SuperUser:
                    return "Super";

                default:
                    return "Unknown";
            }
        }
    }
}
