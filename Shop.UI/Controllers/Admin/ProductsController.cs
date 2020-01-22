using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.ProductsAdmin;
using Shop.Application.Services.Categories;
using Shop.Application.Services.ProductsAdmin;
using Shop.Database;
using System.Threading.Tasks;

namespace Shop.UI.Controllers.Admin
{
    [Route("[controller]")]
    [Authorize(Policy = "Manager")]
    public class ProductsController : Controller
    {
        private ApplicationDbContext _ctx;

        public ProductsController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet("")]
        public IActionResult GetProducts(
            [FromQuery] GetProducts.Request request) =>
                Ok(new GetProducts(_ctx).Do(request));

        [HttpGet("{id}")]
        public IActionResult GetProduct(string id) => Ok(new GetProduct(_ctx).Do(id));

        [HttpPost("")]
        public async Task<IActionResult> CreateProduct(CreateProduct.Request request) => Ok((await new CreateProduct(_ctx).Do(request)));

        [HttpPost("link")]
        public async Task<IActionResult> CreateProductLink(
            [FromBody] CreateProductLink.Request request) =>
                Ok(await new CreateProductLink(_ctx).Do(request));

        [HttpPost("cat")]
        public IActionResult GetCategories() =>
                Ok(new GetCategories(_ctx).DoAdmin());

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(string id) =>
            Ok((await new DeleteProduct(_ctx).Do(id)));

        [HttpDelete("{id}/{targetId}")]
        public async Task<IActionResult> DeleteProductLink(
            string id, string targetId) =>
                Ok(await new DeleteProductLink(_ctx).Do(id, targetId));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(
            UpdateProduct.Request request) =>
                Ok(await new UpdateProduct(_ctx).Do(request));

    }
}
