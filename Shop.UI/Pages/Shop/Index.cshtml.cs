using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Application.Services.Emails;
using Shop.Application.Services.FavouriteLists;
using Shop.Application.Services.PDF;
using Shop.Application.Services.Products.Entities;
using Shop.Database;
using Shop.Domain.Models;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wangkanai.Detection;

namespace Shop.UI.Pages.Shop
{
    public class IndexModel : BasePage
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;
        private EmailSender _emailSender;
        private PDFService _pdfService;
        private readonly ILogger logger;
        public IDevice device;
    


        [ViewData]
        public string Message { get; set; } = "Hi!";
        public IndexModel(ApplicationDbContext ctx, IOptions<Discounts> discounts, EmailSender emailSender, PDFService pdfService, ILogger<IndexModel> logger, IDeviceResolver deviceResolver)
        {
            _ctx = ctx;
            _discounts = discounts;
            _emailSender = emailSender;
            _pdfService = pdfService;
            this.logger = logger;
            device = deviceResolver.Device;
        }

        [BindProperty]
        public AddToCart.Request Input { get; set; }

        [BindProperty]
        public EmailSender.Request email { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<ProductViewModel> DisplayProducts { get; set; }

        public IEnumerable<GetLists.Response> UserLists { get; set; }

        public void OnGet()
        {
            Products = new GetProducts(_ctx, _discounts).Do(new GetProductsQuery { UserId = GetUserId() });
            UserLists = new GetLists(_ctx).Do(GetUserId());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var productView = _pdfService.GetProductAsPdf("");

            return Page();
        }
    }
}
