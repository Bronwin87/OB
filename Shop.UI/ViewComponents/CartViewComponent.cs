using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Application.Services.Cart;
using Shop.Database;
using System;
using System.Linq;
using System.Security.Claims;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;

namespace Shop.UI.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public CartViewComponent(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        public IViewComponentResult Invoke(string view = "Default")
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var sessionId = GetSessionId();

            if (view == "Small")
            {
                var cart = new GetCart(_ctx,_discounts).Do((userId, sessionId));

                var totalValue = cart.Sum(x => x.Value * x.Qty);
                var totalItems = cart.Sum(x => x.Qty);

                //return View(view, $"{totalItems}");              
                return View(view, cart);
            }

            return View(view, new GetCart(_ctx, _discounts).Do((userId, sessionId)));
        }

        private string GetSessionId()
        {
            var SessionId = HttpContext.Session.GetString("SessionId");

            if (!String.IsNullOrEmpty(SessionId))
                return SessionId;

            SessionId = Guid.NewGuid().ToString();
            HttpContext.Session.SetString("SessionId", SessionId);

            return SessionId;
        }
    }
}
