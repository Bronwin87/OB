using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Application.Services.FavouriteLists;
using Shop.UI.Infrastructure;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Controllers
{
    public class FavListController : BaseController
    {
        [HttpGet]
        public IActionResult Get([FromServices] GetListsFull getListsFull) =>
            Ok(getListsFull.Do(GetUserId()));

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromBody] UpdateList.Request request,
            [FromServices] UpdateList updateList) =>
            Ok(await updateList.Do(request));

        public async Task<IActionResult> AddProduct(
            [FromBody] AddProductToList.Request request,
            [FromServices] AddProductToList addProductToList) =>
            Ok(await addProductToList.Do(request));


        [HttpGet("[controller]/[action]/{id}")]
        public async Task<IActionResult> Cart(
            int id,
            [FromServices] GetListsFull getListsFull,
            [FromServices] AddToCart addToCart,
            bool redirectToCheckout = false)
        {
            var mark = GetCartUserMark();
            var list = getListsFull.Do(mark.userId).FirstOrDefault(x => x.Id == id);

            foreach (var product in list.Products)
            {
                await addToCart.Do(new AddToCart.Request
                {
                    UserMark = mark,
                    ProductId = product.Id,
                    Qty = product.Qty == 0 ? 1 : product.Qty
                });
            }

            if (redirectToCheckout)
            {
                return RedirectToPage("/Checkout/Index");
            }

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Product(
            [FromBody] AddToCart.Request request,
            [FromServices] AddToCart addToCart)
        {
            request.UserMark = GetCartUserMark();
            return Ok(addToCart.Do(request));
        }
    }
}
