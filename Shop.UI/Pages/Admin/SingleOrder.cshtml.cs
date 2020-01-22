using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Orders;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Shop.UI.Infrastructure;
using Shop.Domain.Models.Users;
using static Shop.Application.Oders.GetOrder;
using Product = Shop.Domain.Models.Products.Product;
using Shop.Domain.Models.Accounts;

namespace Shop.UI.Pages.Admin
{
    public class SingleOrderModel : BasePage
    {
        public ApplicationDbContext _ctx;
        public SingleOrderModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Response Order { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public IEnumerable<OrderProduct> OrderProductss { get; set; }
        public List<OrderProductForDisplay> ProductList { get; set; }
        public Location Location { get; set; }
        public CostCenter CostCenter { get; set; }
        public void OnGet(string OrderId)
        {
            Order = new Application.Oders.GetOrder(_ctx).Do(OrderId);
            OrderProductss = _ctx.OrderProducts.Include(x =>x.Product).Where(x => x.OrderId == Order.Id);
            if (Order.LocationId != null)
            {
                Location = _ctx.Locations.FirstOrDefault(x => x.Id == Order.LocationId);
            }
            if (Order.CostcenterId != null)
            {
                CostCenter = _ctx.CostCenters.FirstOrDefault(x => x.Id == Order.CostcenterId);
            }



            List<OrderProductForDisplay> ProductListx = new List<OrderProductForDisplay>();
            foreach (var p in OrderProductss)
            {
                OrderProductForDisplay pp = new OrderProductForDisplay
                {

                    Name = p.Product.Name,
                    Unit = p.Product.Unit,
                    Colour = p.Product.Colour,
                    Price = p.Product.Price,
                    ImageUrl = p.Product.ImageUrl,
                    QTY = p.Qty                  
                    
                     
                };
                ProductListx.Add(pp);
               
            }
            ProductList = ProductListx;
            // ApplicationUser = _ctx.Users.FirstOrDefault(x => x.Id = Order.)
        }


        public class OrderProductForDisplay
        {
            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string Unit { get; set; }
            public string Colour { get; set; }
            public decimal ? Price { get; set; }
            public int QTY { get; set; }

        }
    }
}