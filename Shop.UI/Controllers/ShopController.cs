using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Application.Services.Cart;
using Shop.Application.Services.FavouriteLists;
using Shop.Database;
using Shop.Domain.Models;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Server.Kestrel.Https.Internal;
using Microsoft.AspNetCore.Http.Headers;
using System;
using Microsoft.AspNetCore.Http;
using Shop.Application.Oders;
using Shop.Application.Services.Cin7;
using Cin7ApiWrapper.Common;
using Cin7ApiWrapper.Infrastructure;
using Shop.Domain.Enums;
using Shop.Application.Services.Oders;
using Shop.Domain.Models.EmailTemplates;
using Shop.Application.Services.Emails;
using Shop.UI.Utilities;
using Shop.Application.Services.Products.Entities;
using Cin7ApiWrapper.Models;

namespace Shop.UI.Controllers
{
    public class ShopController : BaseController
    {
        [BindProperty]
        public string Name { get; set; }
        private ApplicationDbContext _ctx;
        private EmailSender _emailSender;
        private IOptions<Discounts> _discounts;
        private IViewRenderService _viewRenderService;

        public ShopController(ApplicationDbContext ctx, EmailSender emailSender, IOptions<Discounts> discounts, IViewRenderService viewRenderService)
        {
            _ctx = ctx;
            _emailSender = emailSender;
            _discounts = discounts;
            _viewRenderService = viewRenderService;
        }

        [HttpGet]
        public async Task<IActionResult> SearchProducts(string search, string partialName = "", int partialIndex = 0)
        {
            var products = new GetProductSearch(_ctx).Do(new GetProductSearchQuery
            {
                SearchString = search
            }).ToList();

            if (!string.IsNullOrEmpty(partialName))
            {
                if (partialName.Contains("_FavLine"))
                {
                    foreach (var product in products)
                    {
                        product.ProductAddPartialHtml = await _viewRenderService.RenderToStringAsync(partialName, new FavouriteListProductViewModel()
                        {
                            Index = partialIndex,
                            Id = product.Id,
                            Code = product.Code,
                            ImageUrl = product.ImageUrl,
                            Name = product.Name,
                            Price = product.PriceDec.ToString(),
                            Qty = 1,
                            Uom = product.UOM,
                            TotalPrice = product.PriceDec.ToString(),
                            Deleted = false
                        });
                    }
                }
                else if (partialName.Contains("_QuoteLine"))
                {
                    foreach (var product in products)
                    {
                        product.ProductAddPartialHtml = await _viewRenderService.RenderToStringAsync(partialName, new QuoteProductViewModel()
                        {
                            Index = partialIndex,
                            ProductId = product.Id,
                            ProductCode = product.Code,
                            ImageUrl = product.ImageUrl,
                            ProductName = product.Name,
                            Price = product.PriceDec,
                            Qty = 1,
                            Uom = product.UOM,
                            Deleted = false
                        });
                    }
                }
            }

            return Ok(products);
        }

        [HttpGet]
        public async Task<IActionResult> GetProductListHereForInfiteScroll()
        {
            return View();
        }

        [HttpGet]
        public IActionResult GetFavList()
        {
            var UserLists = new GetLists(_ctx).Do(GetUserId());
            return PartialView("Products/_UserFavList", UserLists);
        }


        public async Task<IActionResult> AddToCart(AddToCart.Request Input)
        {
            Input.UserMark = GetCartUserMark();
            var stockAdded = await new AddToCart(_ctx, _discounts).Do(Input);
            var cart = new GetCart(_ctx, _discounts).Do(GetCartUserMark());
            return PartialView("/Pages/Components/Cart/Small.cshtml", cart);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart([FromBody]UpdateCartQty.Request Input)
        {
            Input.UserMark = GetCartUserMark();
            var stockadded = await new UpdateCartQty(_ctx).Do(Input);
            var cart = new GetCart(_ctx, _discounts).Do(GetCartUserMark());

            var cartItems = await RenderViewAsync("Cart/_CartItems", cart, true);
            var cartSummary = await RenderViewAsync("Cart/_CartSummary", cart, true);
            var cartPartial = await RenderViewAsync("Cart/Small", cart, true);

            return Ok(new { cartItems, cartSummary, cartPartial });
            //return Ok(new { cartItems, cartSummary, qty = cart.Sum(x => x.Qty) });
        }

        public async Task<IActionResult> DeleteFromCart(DeleteFromCart.Request Input)
        {
            Input.UserMark = GetCartUserMark();
            var stockDeleted = await new DeleteFromCart(_ctx).Do(Input);
            return RedirectToPage("/Checkout/Index");
        }

        public async Task<IActionResult> ClearCart()
        {
            DeleteFromCart.Request input = new DeleteFromCart.Request()
            {
                UserMark = GetCartUserMark()
            };
            await new DeleteFromCart(_ctx).DoClear(input);
            return RedirectToPage("/Checkout/Index");
        }

        public async Task<IActionResult> ApproveOrder(UpdateOrderStatus.Request request)
        {
            var x = await new UpdateOrderStatus(_ctx).Do(request);
            return RedirectToPage("/BusinessProfile/Orders");
        }
        public async Task<IActionResult> AddToList(AddProductToList.Request Input)
        {
            await new AddProductToList(_ctx).Do(Input);
            return RedirectToPage("/Shop/Index");
        }

        public async Task<RedirectResult> PushToCin7(string id)
        {
            var order = _ctx.Orders
                .Include(x => x.OrderProducts)

                .Include(x => x.Account)
                   .ThenInclude(x => x.Locations)
                .Include(x => x.Address)
                .Include(x => x.User)
                .FirstOrDefault(x => x.OrderNumber == id);


            if (!(order.Account == null))
            {
                CreateResult result = await new DoCin7SalesOrderReq(_ctx).DoCin7Req(id);

                int? x = result.Id;

                if (result.Success)
                {
                    var updateOrder = new AddCin7IdToOrder(_ctx).Do(result.Id, id);
                }
                return Redirect("/Admin/SingleOrderCin7?orderNumber=" + id + "&cin7Id=" + result.Id);
            }
            else
            {
                CreateResult result = await new DoCin7SalesOrderReq(_ctx).DoCin7ReqIndividual(id);

                var x = result.Id;
                
                if (result.Success)
                {
                    var updateOrder = new AddCin7IdToOrder(_ctx).Do(result.Id, id);                   
                }
                return Redirect("/Admin/SingleOrderCin7?orderNumber=" + id + "&cin7Id=" + result.Id);
            }
                
        }
        public IActionResult Categories()
        {
            var categories = _ctx.MainCategories
                .Include(x => x.SubCategories)
                .ThenInclude(x => x.TertiaryCategories)
                .Select(x => new MCat
                {
                    Id = x.Id,
                    Name = x.Name,
                    SubCategories = x.SubCategories
                        .Select(y => new SCat
                        {
                            Id = y.Id,
                            Name = y.Name,
                            TertiaryCategories = y.TertiaryCategories
                                .Select(z => new TCat
                                {
                                    Id = z.Id,
                                    Name = z.Name
                                })
                        })
                })
                .ToList();

            return Ok(categories);
        }

        public class MCat
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<SCat> SubCategories { get; set; }
        }
        public class SCat
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<TCat> TertiaryCategories { get; set; }

        }
        public class TCat
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public IActionResult Products(
            string category, string categoryId,
            [FromServices] GetProducts getProducts)
        {
            var query = new GetProductsQuery { UserId = GetUserId() };
            if (!string.IsNullOrEmpty(category))
            {
                query.CategoryType = category;
                query.CategoryId = categoryId;
            }
            var products = getProducts.Do(query, 100);
            return Ok(products);
        }

        public async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool isPartial = false)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                viewName = ControllerContext.ActionDescriptor.ActionName;
            }

            ViewData.Model = model;

            using (var writer = new StringWriter())
            {
                IViewEngine viewEngine = HttpContext.RequestServices.GetService(typeof(ICompositeViewEngine)) as ICompositeViewEngine;
                ViewEngineResult viewResult = GetViewEngineResult(viewName, isPartial, viewEngine);

                if (viewResult.Success == false)
                {
                    throw new Exception($"A view with the name {viewName} could not be found");
                }

                ViewContext viewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    ViewData,
                    TempData,
                    writer,
                    new HtmlHelperOptions()
                );

                await viewResult.View.RenderAsync(viewContext);

                return writer.GetStringBuilder().ToString();
            }
        }

        private ViewEngineResult GetViewEngineResult(string viewName, bool isPartial, IViewEngine viewEngine)
        {
            if (viewName.StartsWith("~/"))
            {
                var hostingEnv = HttpContext.RequestServices.GetService(typeof(IHostingEnvironment)) as IHostingEnvironment;
                return viewEngine.GetView(hostingEnv.WebRootPath, viewName, !isPartial);
            }
            else
            {
                return viewEngine.FindView(ControllerContext, viewName, !isPartial);
            }
        }     

        public async Task<ActionResult> UpdateOrderStatus(int orderId, OrderStatus status)
        {
            var updatedOrder = await new UpdateOrderStatus(_ctx).Do(new UpdateOrderStatus.Request()
            {
                OrderId = orderId,
                Status = status,
                AuthoriserId = GetUserId()
            });

            if (updatedOrder.Status == OrderStatus.Complete)
            {
                var me = await _ctx.Users.FindAsync(GetUserId());
                var order = await _ctx.Orders
                                      .Include(x => x.User)
                                      .Include(x => x.Location)
                                      .Include(x => x.Address)
                                      .FirstOrDefaultAsync(x => x.Id == orderId);

                Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", order.User.FirstName) },
                    { "greetingdescription", string.Format("Your order {0} has been approved and will be processed accordingly.", order.OrderNumber) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    { "ordertime", DateTime.UtcNow.ToString("hh:mm tt") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", order.Location.Name, order.Address.Address1, order.Address.Address2, order.Address.City, order.Address.PostCode) }
                };

                Message m = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Order {0} - Your order has been approrved", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = order.User.Email, Name = order.User.FirstName },
                    }
                };

                _emailSender.SendEmailByTemplate(vars, "finalcomfirmation", m);


                Dictionary<string, string> vars2 = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", me.FirstName) },
                    { "greetingdescription", string.Format("Your order {0} has been confirmed and is being processed accordingly.", order.OrderNumber) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    { "ordertime", DateTime.UtcNow.ToString("hh:mm tt") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", order.Location.Name, order.Address.Address1, order.Address.Address2, order.Address.City, order.Address.PostCode) }
                };

                Message m2 = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Awesome - We’ve received the order - {0}", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = me.Email, Name = me.FirstName },
                    }
                };

                _emailSender.SendEmailByTemplate(vars2, "finalcomfirmation", m2);

                string cmsLink = Url.Action("SingleOrder", "Admin", new { id = order.OrderNumber }, Request.Scheme);

                Dictionary<string, string> vars3 = new Dictionary<string, string>
                {
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", DateTime.UtcNow.ToString("dd MMM yyyy") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", order.Location.Name, order.Location.Address.Address1, order.Location.Address.Address2, order.Location.Address.City, order.Location.Address.PostCode) },
                    { "cmslink", string.Format("<a href='{0}'>{0}</a>", cmsLink) },
                };

                Message m3 = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("OfficeBox Order Placed - {0}", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = "orders@officebox.co.za", Name = "OfficeBox" }
                    }
                };

                _emailSender.SendEmailByTemplate(vars3, "internal-order-confirmation", m3);
            }
            else if (updatedOrder.Status == OrderStatus.Declined)
            {
                var me = await _ctx.Users.FindAsync(GetUserId());
                var order = await _ctx.Orders
                                      .Include(x => x.User)
                                      .Include(x => x.Location)
                                      .Include(x => x.Address)
                                      .FirstOrDefaultAsync(x => x.Id == orderId);

                Dictionary<string, string> vars = new Dictionary<string, string>
                {
                    { "greeting", string.Format("Hi {0}", order.User.FirstName) },
                    { "greetingdescription", string.Format("Please note {0} {1} has declined the order.", me.FirstName, me.LastName) },
                    { "ordernumber", order.OrderNumber },
                    { "orderprice", string.Format("R {0}", order.SubTotal.ToString("N2")) },
                    { "orderdate", order.Created.ToString("dd MMM yyyy") },
                    { "ordertime", order.Created.ToString("hh:mm:ss") },
                    { "orderaddress", string.Format("<strong>{0}</strong><br/>{1}<br/>{2}<br/>{3}<br/>{4}", order.Location.Name, order.Address.Address1, order.Address.Address2, order.Address.City, order.Address.PostCode) }
                };

                Message m = new Message
                {
                    FromEmail = "orders@officebox.co.za",
                    FromName = "Officebox",
                    Subject = string.Format("Order Number {0} - Declined", order.OrderNumber),
                    To = new To[]
                    {
                        new To { Email = order.User.Email, Name = order.User.FirstName },
                    }
                };

                _emailSender.SendEmailByTemplate(vars, "finalcomfirmation", m);
            }

            return RedirectToAction("Orders", "BusinessProfile");
        }
    }
}
