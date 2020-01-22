using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Orders;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Shop.UI.Infrastructure;

namespace Shop.UI.Pages.Admin
{
    public class OrderModel : BasePage
    {
        private ApplicationDbContext _ctx;

        public OrderModel(ApplicationDbContext ctx)
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
                .ToList();
        }
    }
}