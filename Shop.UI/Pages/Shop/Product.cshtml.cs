using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Application.Services.Products.Entities;
using Shop.Database;
using Shop.UI.Infrastructure;
using System.Threading.Tasks;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using Shop.Application;
using Shop.UI.Utilities;


namespace Shop.UI.Pages.Shop
{
    public class ProductModel : BasePage
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public ProductModel(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        [BindProperty]
        public AddToCart.Request Input { get; set; }

        public GetProduct.ProductViewModel Product { get; set; }
        public IEnumerable<ProductViewModel> RelatedProducts { get; set; }

        public async Task<IActionResult> OnGet(string Id)
        {
            Product = await new GetProduct(_ctx,_discounts).Do(Id);
            RelatedProducts = new GetProduct(_ctx, _discounts).GetRelatedProducts(Id);

            if (Product == null)
                return RedirectToPage("Index");
            else
                return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            Input.UserMark = GetCartUserMark(); // TODO
            var stockAdded = await new AddToCart(_ctx,_discounts).Do(Input);

            if (stockAdded)
                return RedirectToPage("/Checkout/Index");
            else
                return Page();
        }
    }
}