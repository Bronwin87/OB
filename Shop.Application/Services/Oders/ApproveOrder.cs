using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.Oders
{
    public class UpdateOrderStatus
    {
        private ApplicationDbContext _ctx;
        public UpdateOrderStatus(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public class Request
        {
            public int OrderId { get; set; }
            public OrderStatus Status { get; set; }
            public string AuthoriserId { get; set; }
        }


        public async Task<Order> Do(Request request)
        {
            var order = await _ctx.Orders.FindAsync(request.OrderId);

            order.Status = request.Status;
            order.AuthorizerId = request.AuthoriserId;
            order.AuthorizedDate = DateTime.UtcNow;

            await _ctx.SaveChangesAsync();
            return order;
        }
    }

    
}
