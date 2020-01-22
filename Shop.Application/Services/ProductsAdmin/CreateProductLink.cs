using Shop.Database;
using Shop.Domain.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Shop.Application.Services.ProductsAdmin
{
    public class CreateProductLink
    {
        private ApplicationDbContext _ctx;

        public CreateProductLink(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public string Id { get; set; }
            public IEnumerable<string> Links { get; set; }
            public bool Link { get; set; }
            public bool Alt { get; set; }
            public bool Value { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var type = ProductLinkType.Relation;

            if (request.Alt)
                type = ProductLinkType.Alternative;
            else if (request.Value)
                type = ProductLinkType.ValueAdded;

            foreach (var link in request.Links)
                _ctx.ProductLinks.Add(new Domain.Models.Products.ProductLink
                {
                    RootId = request.Id,
                    TargetId = link,
                    Type = type
                });

            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
