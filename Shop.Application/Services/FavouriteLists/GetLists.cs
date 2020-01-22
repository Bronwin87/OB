using Shop.Database;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.Services.FavouriteLists
{
    public class GetLists
    {
        private ApplicationDbContext _ctx;

        public GetLists(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Response
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }

        public IEnumerable<Response> Do(string userId) =>
            _ctx.FavouriteLists
                .Where(x => x.UserId == userId)
                .Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name
                })
                .ToList();
    }
}
