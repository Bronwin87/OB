using Shop.Database;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.FavouriteLists
{
    public class DeleteList
    {
        private ApplicationDbContext _ctx;

        public DeleteList(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public async Task Do(int id)
        {
            var list = _ctx.FavouriteLists.FirstOrDefault(x => x.Id == id);

            foreach (var item in list.FavouriteProducts.ToList())
            {
                _ctx.FavouriteListProducts.Remove(item);
            }

            _ctx.FavouriteLists.Remove(list);

            await _ctx.SaveChangesAsync();
        }
    }
}
