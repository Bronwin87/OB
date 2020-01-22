using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Shop.UI.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Shop.Application.Services.Timezone;

namespace Shop.UI.Pages.BusinessProfile.Orders
{
    [Authorize]
    public class IndexModel : BasePage
    {
        private ApplicationDbContext _ctx;

        public IndexModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Order> Orders { get; set; }
        public bool CanEdit { get; private set; }
        public bool IsSuper { get; private set; }

        public void OnGet()
        {
            IsSuper = User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "superuser";

            var accountId = _ctx.AccountUsers
               .FirstOrDefault(x => x.UserId == GetUserId() && x.Active)
               .AccountId;

            Orders = _ctx.Orders
                .Include(x => x.Address)
                .Include(x => x.User)
                .Include(x => x.Location)
                    .ThenInclude(x => x.Authorizer)
                .Include(x => x.CostCenter)
                    .ThenInclude(x => x.Authorizer)
                .Include(x => x.OrderProducts)
                    .ThenInclude(x => x.Product)
                .Where(x => x.AccountId == accountId)
                .ToList();

            foreach(var order in Orders)
            {
                order.Created = order.Created.ConvertTimeToLocal();
                order.AuthorizedDate =  order.AuthorizedDate.ConvertTimeToLocal();
            }
        }
    }
}