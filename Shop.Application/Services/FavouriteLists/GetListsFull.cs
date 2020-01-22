using Microsoft.EntityFrameworkCore;
using Shop.Database;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.Services.FavouriteLists
{
    public class GetListsFull
    {
        private ApplicationDbContext _ctx;

        public GetListsFull(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Response
        {
            public int Id { get; set; }

            public string Name { get; set; }
            public string TotalPrice { get; set; }
            public decimal TotalPriceDec { get; set; }
            public string TotalQty { get; set; }

            public IEnumerable<ListProduct> Products { get; set; }

            public bool Show { get; set; }
            public bool Active { get; set; }
        }

        public class ListProduct
        {
            public string Id { get; set; }

            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Uom { get; set; }
            public string Price { get; set; }
            public int Qty { get; set; }
            public string TotalPrice { get; set; }

            public bool Deleted { get; set; }
        }

        public IEnumerable<Response> Do(string userId) =>
            _ctx.FavouriteLists
                .Include(x => x.FavouriteProducts)
                    .ThenInclude(x => x.Product)
                .Where(x => x.UserId == userId)
                .Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    TotalPrice = (x.FavouriteProducts.Sum(y => y.Qty * y.Product.Price) ?? 0).ToString("N2"),
                    TotalPriceDec = (x.FavouriteProducts.Sum(y => y.Qty * y.Product.Price) ?? 0),
                    //TotalQty = x.FavouriteProducts.Sum(y => y.Qty).ToString("N2"),
                    TotalQty = x.FavouriteProducts.Count().ToString(),

                    Products = x.FavouriteProducts.Select(y => new ListProduct
                    {
                        Id = y.ProductId,
                        ImageUrl = y.Product.ImageUrl,
                        Name = y.Product.Name,
                        Code = y.Product.ExternalId,
                        Uom = y.Product.Unit,
                        Price = (y.Product.Price ?? 0).ToString("N2"),
                        Qty = y.Qty,
                        TotalPrice = (y.Qty * (y.Product.Price ?? 0)).ToString("N2"),

                        Deleted = false
                    }),

                    Show = true,
                    Active = false
                })
                .ToList();
    }
}
