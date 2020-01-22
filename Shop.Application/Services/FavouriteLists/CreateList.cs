using Shop.Database;
using Shop.Domain.Models.Users;
using System.Threading.Tasks;

namespace Shop.Application.Services.FavouriteLists
{
    public class CreateList
    {
        private ApplicationDbContext _ctx;

        public CreateList(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public string UserId { get; set; }
            public string Name { get; set; }
        }

        public async Task Do(Request request)
        {
            var list = new FavouriteList
            {
                Name = request.Name,
                UserId = request.UserId
            };

            _ctx.Add(list);

            await _ctx.SaveChangesAsync();
        }
    }
}
