using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Application.Services.Quotes;
using Shop.UI.Infrastructure;
using System.Threading.Tasks;

namespace Shop.UI.Controllers
{
    public class QuotesController : BaseController
    {
        private QuotesService _quotesService;

        public QuotesController(QuotesService quotesService)
        {
            _quotesService = quotesService;
        }

        [HttpGet]
        public IActionResult Get() =>
           Ok(_quotesService.GetQuotes(GetUserId()));

        [HttpPost]
        public async Task<IActionResult> Create(QuotesService.Request request)
        {
            request.UserId = GetUserId();
            var result = await _quotesService.CreateQuote(request);
            return RedirectToPage("/BusinessProfile/Quotes/Index");
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] QuotesService.UpdateRequest request) =>
                Ok(await _quotesService.UpdateQuote(request));

        [HttpPost("[controller]/[action]/{id}")]
        public async Task<IActionResult> Cart(
           int id,
           [FromServices] AddToCart addToCart)
        {
            var mark = GetCartUserMark();
            var list = _quotesService.GetQuote(id);

            foreach (var product in list.QuoteProducts)
            {
                await addToCart.Do(new AddToCart.Request
                {
                    UserMark = mark,
                    ProductId = product.ProductId,
                    Qty = product.Qty
                });
            }

            return Ok();
        }
    }
}
