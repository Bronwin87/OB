using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Admin
{
    public class DiscountsModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public DiscountsModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public IEnumerable<Account> Accounts { get; set; }
        public List<Product> Products { get; private set; }

        [BindProperty]
        public Discount Input { get; set; }

        public void OnGet()
        {
            Accounts = _ctx.Accounts.ToList();
            Products = _ctx.Products.Take(20).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            _ctx.Discounts.Add(new Domain.Models.Products.Discount
            {
                AccountId = Input.AccountId,
                ProductId = Input.ProductId,
                DiscountType = Input.DiscountType,
                DiscountPercent = Input.Amount,
                DiscountValue = Input.Amount,
            });

            await _ctx.SaveChangesAsync();

            return RedirectToPage("/Admin/Discounts");
        }
    }

    public class Discount
    {
        public int AccountId { get; set; }
        public string ProductId { get; set; }
        public DiscountType DiscountType { get; set; }
        public int Amount { get; set; }
    }
}