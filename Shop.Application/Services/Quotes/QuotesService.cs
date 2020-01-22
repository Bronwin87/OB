using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Quotes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Services.Quotes
{
    public class QuotesService
    {
        private ApplicationDbContext _ctx;


        public QuotesService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public string UserId { get; set; }
            public string QuoteName { get; set; }
        }

        public async Task<bool> CreateQuote(Request request)
        {
            var cart = _ctx.Carts
                   .Include(x => x.Products)
                   .ThenInclude(x => x.Product)
                   .FirstOrDefault(x => x.UserId == request.UserId);

            if (cart == null)
            {
                return false;
            }

            var user = _ctx.AccountUsers.FirstOrDefault(x => x.UserId == request.UserId);

            if (user == null)
            {
                return false;
            }

            var quote = new Quote
            {
                Created = DateTime.UtcNow,
                Name = request.QuoteName,
                AccountId = user.AccountId,
                UserId = request.UserId
            };

            foreach (var product in cart.Products)
            {
                quote.QuoteProducts.Add(new QuoteProduct()
                {
                    ProductId = product.ProductId,
                    Qty = product.Qty
                });
            }

            _ctx.Quotes.Add(quote);

            // Once the quote is created, remove all the items from the cart.
            cart.Products.Clear();
            await _ctx.SaveChangesAsync();

            return true;
        }

        public class UpdateRequest
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<RequestProduct> Products { get; set; }
        }

        public class RequestProduct
        {
            public string Id { get; set; }
            public int Qty { get; set; }
            public bool Deleted { get; set; }
        }

        public async Task<bool> UpdateQuote(UpdateRequest request)
        {
            var list = _ctx.Quotes
                .Include(x => x.QuoteProducts)
                .FirstOrDefault(x => x.Id == request.Id);

            if (!string.IsNullOrEmpty(request.Name))
            {
                list.Name = request.Name;
            }
            list.QuoteProducts = new List<QuoteProduct>();

            foreach (var p in request.Products.Where(x => !x.Deleted))
            {
                list.QuoteProducts.Add(new QuoteProduct
                {
                    ProductId = p.Id,
                    Qty = p.Qty
                });
            }

            return await _ctx.SaveChangesAsync() > 0;
        }

        public Quote GetQuote(int id)
        {
            return _ctx.Quotes
                    .Include(x => x.QuoteProducts)
                    .ThenInclude(x => x.Product)
                    .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Response> GetQuotes(string userId)
        {
            return _ctx.Quotes
                .Include(x => x.QuoteProducts)
                .ThenInclude(x => x.Product)
                .Where(x => x.UserId == userId)
                .Select(x => new Response
                {
                    Id = x.Id,
                    Name = x.Name,
                    DateCreated = x.Created.ToString("dd/MM/yyyy"),
                    TotalPrice = (x.QuoteProducts.Sum(y => y.Qty * y.Product.Price) ?? 0).ToString("N2"),
                    TotalQty = x.QuoteProducts.Sum(y => y.Qty).ToString("N2"),

                    Products = x.QuoteProducts.Select(y => new ListProduct
                    {
                        Id = y.ProductId,
                        ImageUrl = y.Product.ImageUrl,
                        Name = y.Product.Name,
                        Code = y.Product.ExternalId,
                        Uom = y.Product.Unit,
                        Price = (y.Product.Price ?? 0).ToString("N2"),
                        Qty = y.Qty,
                        TotalPrice = (y.Qty * (y.Product.Price ?? 0)).ToString("N2"),

                        Deleted = false
                    }),

                    Show = true,
                    Active = false
                })
                .ToList();
        }

        public class Response
        {
            public int Id { get; set; }

            public string DateCreated { get; set; }
            public string Name { get; set; }
            public string TotalPrice { get; set; }
            public string TotalQty { get; set; }

            public IEnumerable<ListProduct> Products { get; set; }

            public bool Show { get; set; }
            public bool Active { get; set; }
        }

        public class ListProduct
        {
            public string Id { get; set; }

            public string ImageUrl { get; set; }
            public string Name { get; set; }
            public string Code { get; set; }
            public string Uom { get; set; }
            public string Price { get; set; }
            public int Qty { get; set; }
            public string TotalPrice { get; set; }

            public bool Deleted { get; set; }
        }
    }
}
