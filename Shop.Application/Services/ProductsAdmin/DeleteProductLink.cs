using Shop.Database;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.ProductsAdmin
{
    public class DeleteProductLink
    {
        private ApplicationDbContext _ctx;

        public DeleteProductLink(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public async Task<bool> Do(string rootId, string targetId)
        {
            var link = _ctx.ProductLinks
                .FirstOrDefault(x => x.RootId == rootId
                && x.TargetId == targetId);

            if (link != null)
                _ctx.ProductLinks.Remove(link);

            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
