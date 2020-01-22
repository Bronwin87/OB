using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.Timezone;
using Shop.Database;
using Shop.Domain.Models.Orders;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using System.Linq;

namespace Shop.UI.Pages.Profile
{
    public class OrderHistoryModel : BasePage
    {
        private ApplicationDbContext _ctx;

        public OrderHistoryModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Order> Orders { get; set; }

        public void OnGet()
        {
            Orders = _ctx.Orders
                .Include(x => x.Address)
                .Include(x => x.User)
                .Include(x => x.OrderProducts)
                .ThenInclude(x => x.Product)
                .Where(x => x.UserId == GetUserId())
                .ToList();

            foreach (var order in Orders)
            {
                order.Created = order.Created.ConvertTimeToLocal();
                order.AuthorizedDate = order.AuthorizedDate.ConvertTimeToLocal();
            }
        }
    }
}