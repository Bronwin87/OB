using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Database;
using Shop.UI.Infrastructure;
using Shop.UI.Utilities;
using System.Collections.Generic;
using System.Threading.Tasks;
using Shop.Application.Services.Products.Entities;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;
using Wangkanai.Detection;

namespace Shop.UI.Pages
{
    public class IndexModel : BasePage
    {
        private ApplicationDbContext _ctx;
        private CookiesHelper _cookiesHelper;
        public IDevice device;
        public IndexModel(ApplicationDbContext ctx, CookiesHelper cookiesHelper, IDeviceResolver deviceResolver)
        {
            _ctx = ctx;
            _cookiesHelper = cookiesHelper;
            device = deviceResolver.Device;
        }

        [BindProperty]
        public AddToCart.Request Input { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }

        public async Task<IActionResult> OnGet([FromServices] GetProducts getProducts, bool skipCookie = false)
        {
            string urlAnterior = Request.Headers["Referer"].ToString();

            if (!(device.Type == DeviceType.Mobile || device.Type == DeviceType.Tablet))
            {
                if (!skipCookie)
                {
                    var cookieValue = _cookiesHelper.Get("PreviouslyLoggedInUser");

                    if (cookieValue != null)
                    {
                        if (cookieValue == "true")
                        {
                            return RedirectToPage("/shop/index");
                        }
                    }
                }
            }
            else
            {
                // Why? I don't quite understand. - Carel
                foreach (var cookie in Request.Cookies.Keys)
                {
                    if (cookie.Contains("PreviouslyLoggedInUser"))
                    {
                        Response.Cookies.Delete(cookie);
                    }
                }
            }

            Products = getProducts.Do(
                new GetProductsQuery
                {
                    UserId = GetUserId()
                });

            return Page();
        }
    }
}
