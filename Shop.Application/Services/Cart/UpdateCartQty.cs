using System;
using System.Collections.Generic;
using System.Text;
using Shop.Database;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.Cart;

namespace Shop.Application.Services.Cart
{
    public class UpdateCartQty
    {
        private ApplicationDbContext _context;

        public UpdateCartQty(ApplicationDbContext context)
        {
            _context = context;
        }

        public class Request
        {
            public (string userId, string sessionId) UserMark { get; set; }
            public string ProductId { get; set; }
            public int Qty { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var cart = _context.Carts
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => (x.UserId == request.UserMark.userId
                        && !string.IsNullOrEmpty(request.UserMark.userId))
                    || x.SessionId == request.UserMark.sessionId);

            if (cart != null)
            {
                decimal discountEligibleTotal = 0m;
                decimal noDiscountTotal = 0m;
                cart.Discounted = false;
                cart.Discount = 0;
                cart.Delivery = 60;
                cart.DiscountThreshold = 0;

                var exitingProduct = cart.Products.FirstOrDefault(x => x.ProductId == request.ProductId);

                if (exitingProduct != null)
                {
                    exitingProduct.Qty = request.Qty;
                }
                else
                {
                    var newProduct = _context.Products.Find(request.ProductId);
                    cart.Products.Add(new CartProduct
                    {
                        ProductId = request.ProductId,
                        Qty = request.Qty,
                        Product = newProduct
                    });
                }

                cart.ProductCount = cart.Products.Sum(x => x.Qty);
                cart.OriginalSubtotal = Convert.ToDecimal(cart.Products.Sum(x => x.Qty * x.Product.Price));

                if (cart.OriginalSubtotal >= 650)
                {
                    cart.Delivery = 0;
                }

                //threshold discounts
                foreach (var p in cart.Products)
                {
                   // if (p.Product.NoDiscount != true)
                   // {
                        discountEligibleTotal += ((p.Product.Price ?? 0m) * p.Qty);

                    if (p.Product.NoDiscount == true)
                    {
                        noDiscountTotal += ((p.Product.Price ?? 0m) * p.Qty);
                    }
                    //}
                }

                if (discountEligibleTotal >= 1500 && discountEligibleTotal < 2500)
                {
                    //apply 2.5% discounts
                    cart.DiscountThreshold = 2.5m;
                    cart.Discount = (discountEligibleTotal - noDiscountTotal) * 0.025m;
                }
                else if (cart.OriginalSubtotal >= 2500)
                {
                    //apply 5% discounts
                    cart.DiscountThreshold = 5m;
                    cart.Discount = (discountEligibleTotal - noDiscountTotal) * 0.05m;
                }

                if (cart.Discount > 0)
                {
                    cart.Discounted = true;
                }

                cart.Vat = CalculateVat((cart.OriginalSubtotal + cart.Delivery) - cart.Discount);
                cart.SubTotal = (cart.OriginalSubtotal + cart.Delivery + cart.Vat) - cart.Discount;

                //return await _context.SaveChangesAsync() > 0;
            }
            var newCart = new Domain.Models.Cart.Cart
            {
                UserId = request.UserMark.userId,
                SessionId = request.UserMark.sessionId,
            };

            newCart.Products.Add(new CartProduct
            {
                ProductId = request.ProductId,
                Qty = request.Qty
            });

            _context.Carts.Add(newCart);
            return await _context.SaveChangesAsync() > 0;
        }

        public decimal CalculateVat(decimal value)
        {
            return (value / 100) * 15;
        }
    }
}
