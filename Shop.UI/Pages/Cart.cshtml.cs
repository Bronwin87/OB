using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Database;
using Shop.UI.Infrastructure;
using System.Collections.Generic;

namespace Shop.UI.Pages
{
    public class CartModel : BasePage
    {
        private ApplicationDbContext _ctx;

        public CartModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<GetCart.Response> Cart { get; set; }

        public IActionResult OnGet()
        {
          //  Cart = new GetCart(_ctx).Do(GetCartUserMark());

            return Page();
        }
    }
}