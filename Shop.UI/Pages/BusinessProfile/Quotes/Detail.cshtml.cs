using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Application.Cart;
using Shop.Application.Services.Products.Entities;
using Shop.Application.Services.Quotes;
using Shop.UI.Infrastructure;

namespace Shop.UI.Pages.BusinessProfile.Quotes
{
    public class DetailModel : BasePage
    {
        private readonly QuotesService _quotesService;
        private readonly AddToCart _addToCart;

        public DetailModel(QuotesService quotesService, AddToCart addToCart)
        {
            this._quotesService = quotesService;
            this._addToCart = addToCart;
        }

        [BindProperty]
        public QuoteViewModel QuoteModel { get; set; }

        public void OnGet(int id)
        {
            var quote = _quotesService.GetQuote(id);
            QuoteModel = new QuoteViewModel()
            {
                Id = quote.Id,
                Name = quote.Name,
                DateCreated = quote.Created.ToString("dd/MM/yyyy"),
                QuoteProducts = quote.QuoteProducts.Select(qp => new QuoteProductViewModel()
                {
                    ProductId = qp.ProductId,
                    ProductName = qp.Product.Name,
                    ProductCode = qp.Product.ExternalId,
                    ImageUrl = qp.Product.ImageUrl,
                    Uom = qp.Product.Unit,
                    Price = qp.Product.Price ?? 0m,
                    Qty = qp.Qty
                }).ToList()
            };
        }

        public async Task<IActionResult> OnPost(string SubmitAction)
        {
            if (SubmitAction == "AddCart")
            {
                var mark = GetCartUserMark();

                foreach (var item in QuoteModel.QuoteProducts)
                {
                    await _addToCart.Do(new AddToCart.Request()
                    {
                        UserMark = mark,
                        ProductId = item.ProductId,
                        Qty = item.Qty
                    });
                }
            }
            else if (SubmitAction == "UpdateChanges")
            {
                await _quotesService.UpdateQuote(new QuotesService.UpdateRequest()
                {
                    Id = QuoteModel.Id,
                    Name = QuoteModel.Name,
                    Products = QuoteModel.QuoteProducts.Select(p => new QuotesService.RequestProduct()
                    {
                        Id = p.ProductId,
                        Deleted = p.Deleted,
                        Qty = p.Qty
                    })
                });
            }
            return RedirectToPage("/BusinessProfile/Quotes/Detail", new { id = QuoteModel.Id });
        }

        public class QuoteViewModel
        {
            public QuoteViewModel()
            {
                QuoteProducts = new List<QuoteProductViewModel>();
            }

            public int Id { get; set; }
            public string DateCreated { get; set; }
            public string Name { get; set; }
            public int TotalQty
            {
                get
                {
                    return this.QuoteProducts.Sum(p => p.Qty);
                }
            }
            public decimal TotalPrice
            {
                get
                {
                    return this.QuoteProducts.Sum(p => p.TotalPrice);
                }
            }
            public decimal Delivery
            {
                get
                {
                    return this.QuoteProducts.Any() ? (this.TotalPrice > 650m ? 0m : 60.00m) : 0m;
                }
            }
            public string DeliveryText
            {
                get
                {
                    return this.Delivery == 0 ? "Free" : "R " + this.Delivery.ToString("N2");
                }
            }
            public decimal SubTotal
            {
                get
                {
                    return this.TotalPrice + this.Delivery;
                }
            }
            public decimal Vat
            {
                get
                {
                    return SubTotal * 0.15m;
                }
            }
            public decimal TotalPriceIncl
            {
                get
                {
                    return SubTotal + Vat;
                }
            }

            public List<QuoteProductViewModel> QuoteProducts { get; set; }
        }
    }
}