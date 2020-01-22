using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.OrdersAdmin;
using Shop.Database;
using System.Threading.Tasks;
using Shop.Application.Services.Cin7;
using Cin7ApiWrapper.Common;
using Cin7ApiWrapper.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Shop.UI.Controllers.Admin
{
    [Route("[controller]")]
    [Authorize(Policy = "Manager")]
    public class OrdersController : Controller
    {
        private ApplicationDbContext _ctx;

        public OrdersController(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [HttpGet("")]
        public IActionResult GetOrders(
            [FromQuery] GetOrders.Request request) =>
                Ok(new GetOrders(_ctx).Do(request));

        [HttpGet("{id}")]
        public IActionResult GetOrder(int id) => Ok(new GetOrder(_ctx).Do(id));

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrder(int id) => Ok((await new UpdateOrder(_ctx).Do(id)));

        //public async Task<IActionResult> UploadToCin7(string id) => Ok((await new DoCin7SalesOrderReq(_ctx).DoCin7Req (id)));
        
        public async Task<JsonResult> GetOrdersAdmin()
        {
            return Json(true);
        }
    }
}
