using Microsoft.EntityFrameworkCore;
using Shop.Database;
using System.Collections.Generic;
using System.Linq;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using System.Text.RegularExpressions;

namespace Shop.Application.Cart
{
    public class GetCart
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public GetCart(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        public class Response
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string ImageUrl { get; set; }
            public string Code { get; set; }
            public string Unit { get; set; }
            public string Price { get; set; }
            public decimal Value { get; set; }
            public int Qty { get; set; }
            public string Total { get; set; }

            public int StockId { get; set; }
            public string MainCategoryId { get; set; }

            public string SubCategoryId { get; set; }

            public string TertiaryCategoryId { get; set; }
            public bool NoDiscount { get; set; }

            public decimal CartSubTotal { get; set; }
            public decimal CartShipping { get; set; }
            public decimal ProductThresholdDiscount { get; set; }
            public decimal CartTotal { get; set; }

        }

        public IEnumerable<Response> Do(
            (string userId, string sessionId) UserMark)
        {
            var cart = _ctx.Carts
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .Include(x => x.User)
                .FirstOrDefault(x => 
                   (x.UserId == UserMark.userId && !string.IsNullOrEmpty(UserMark.userId))
                   || (x.SessionId == UserMark.sessionId));

            if (cart == null)
                return new List<Response>();

            var response = cart.Products
                .Select(x => new Response
                {
                    Id = x.ProductId,
                    Name = x.Product.Name,
                    Code = x.Product.ExternalId,
                    ImageUrl = x.Product.ImageUrl,
                    Unit = x.Product.Unit,
                    Price = $"R{(x.Product.Price.HasValue ? x.Product.Price.Value.ToString("N2") : "")}",
                    Total = $"R{(x.Product.Price.HasValue ? (x.Product.Price.Value * x.Qty).ToString("N2") : "")}",
                    Value = (x.Product.Price.HasValue ? x.Product.Price.Value : 0),
                    Qty = x.Qty,
                    MainCategoryId = x.Product.MainCategoryId,
                    SubCategoryId = x.Product.SubCategoryId,
                    TertiaryCategoryId = x.Product.TertiaryCategoryId,
                    NoDiscount = x.Product.NoDiscount ?? false,

                    CartSubTotal = x.Cart.SubTotal,
                    ProductThresholdDiscount = x.Cart.Discount,
                })
                .ToList();
            return response;
        }
    }
}
