using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Users;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.FavouriteLists
{
    public class AddProductToList
    {
        private ApplicationDbContext _ctx;

        public AddProductToList(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public int ListId { get; set; }
            public string ProductId { get; set; }
            public int Qty { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var list = _ctx.FavouriteLists
                .Include(x => x.FavouriteProducts)
                .FirstOrDefault(x => x.Id == request.ListId);

            var product = list.FavouriteProducts.FirstOrDefault(x => x.ProductId == request.ProductId);

            if (product != null)
            {
                product.Qty += request.Qty;
            }
            else
            {
                list.FavouriteProducts.Add(new FavouriteListProduct
                {
                    ProductId = request.ProductId,
                    Qty = request.Qty
                });
            }

            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
