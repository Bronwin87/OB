using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Database;
using System.Linq;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using Shop.Application.Services.FavouriteLists;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;
using Shop.Application.Services.Products.Entities;

namespace Shop.UI.Pages.Shop
{
    public class VAIndexModel : BasePage
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public VAIndexModel(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        [BindProperty]
        public AddToCart.Request Input { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<ProductViewModel> DisplayProducts { get; set; }

        public IEnumerable<Application.Services.FavouriteLists.GetLists.Response> UserLists { get; set; }


        public void OnGet()
        {
            Products = new GetVAProducts(_ctx, _discounts).Do(new GetVAProductsQuery { UserId = GetUserId(), ValueAddedProduct = true });

            UserLists = new GetLists(_ctx).Do(GetUserId());
        }
    }
}