using Microsoft.EntityFrameworkCore;
using Shop.Database;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Cart
{
    public class SetCartAddress
    {
        private ApplicationDbContext _ctx;

        public SetCartAddress(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public (string userId, string sessionId) UserMark { get; set; }
            public int AddressId { get; set; }
            public int CostCenterId { get; set; }
            public string OrderReference { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var cart = _ctx.Carts
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => (x.UserId == request.UserMark.userId
                        && !string.IsNullOrEmpty(request.UserMark.userId))
                    || x.SessionId == request.UserMark.sessionId);

            cart.AddressId = request.AddressId;

            if (request.CostCenterId > 0)
                cart.CostCenterId = request.CostCenterId;

            return await _ctx.SaveChangesAsync() > 0;
        }
    }
}
