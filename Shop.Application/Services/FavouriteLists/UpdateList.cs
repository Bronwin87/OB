using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.FavouriteLists
{
    public class UpdateList
    {
        private ApplicationDbContext _ctx;

        public UpdateList(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<RequestProduct> Products { get; set; }
        }

        public class RequestProduct
        {
            public string Id { get; set; }
            public int Qty { get; set; }
            public bool Deleted { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var list = _ctx.FavouriteLists
                .Include(x => x.FavouriteProducts)
                .FirstOrDefault(x => x.Id == request.Id);

            if (!string.IsNullOrEmpty(request.Name))
            {
                list.Name = request.Name;
            }
            list.FavouriteProducts = new List<FavouriteListProduct>();

            foreach (var p in request.Products.Where(x => !x.Deleted))
            {
                if (!list.FavouriteProducts.Any(lp => lp.ProductId == p.Id))
                {
                    list.FavouriteProducts.Add(new FavouriteListProduct
                    {
                        ProductId = p.Id,
                        Qty = p.Qty
                    });
                }
            }

            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
