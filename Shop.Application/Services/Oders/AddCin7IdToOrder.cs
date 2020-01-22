using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.Oders
{
    public class AddCin7IdToOrder
    {
        private ApplicationDbContext _ctx;
        public AddCin7IdToOrder(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Application.Oders.GetOrder.Response Order { get; set; }

        public class Request
        {
            public string OrderId { get; set; }
            public int Cin7SalesOrderId { get; set; }
            
        }
        
        public  Order Do(int? Cin7Id, string orderNumber)
        {
            var order =  _ctx.Orders.FirstOrDefault(o => o.OrderNumber == orderNumber);  
            
            order.Cin7ID = Cin7Id;
             _ctx.SaveChanges();
            return order;
        }
    }

    
}
