using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Database;
using Shop.Domain.Models.Cart;
using System.Linq;
using System.Threading.Tasks;
using Shop.Domain.Models;
using System;
using System.Collections.Generic;

namespace Shop.Application.Cart
{
    public class AddToCart
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public AddToCart(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        public class Request
        {
            public (string userId, string sessionId) UserMark { get; set; }
            public string ProductId { get; set; }
            public int Qty { get; set; }
        }

        public async Task<bool> Do(Request request)
        {
            var cart = _ctx.Carts
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
                    exitingProduct.Qty += request.Qty;
                }
                else
                {
                    var newProduct = _ctx.Products.Find(request.ProductId);
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
                    //not required nodiscount items add to the threshold
                    // if (p.Product.NoDiscount != true)
                    // {
                    /////////////////////////////////////////////////////
                    /////////WE can just use originalsubtotal now actually
                    discountEligibleTotal += ((p.Product.Price ?? 0m) * p.Qty);
                    // }
                    //total of products not eligible for discount
                    if (p.Product.NoDiscount == true)
                    {
                        noDiscountTotal += ((p.Product.Price ?? 0m) * p.Qty);
                    }
                }

                if (discountEligibleTotal >= 1500 && discountEligibleTotal < 2500)
                {
                    //apply 2.5% discounts
                    cart.DiscountThreshold = 2.5m;

                    //we subtract the nodiscount total from original subtotal before we get the discount amount, no discoutn items help with threshold but no discounts can be applied to them
                    cart.Discount = (discountEligibleTotal - noDiscountTotal) * 0.025m;

                    //////////////////////////////////////ORIGINALLY
                    //cart.Discount = discountEligibleTotal * 0.025m;
                }
                else if (cart.OriginalSubtotal >= 2500)
                {
                    //apply 5% discounts
                    cart.DiscountThreshold = 5m;

                    //remove nodiscount items total from subtotal before we get the discount
                    cart.Discount = (discountEligibleTotal - noDiscountTotal) * 0.05m;
                }

                if (cart.Discount > 0)
                {
                    cart.Discounted = true;
                }

                cart.Vat = CalculateVat((cart.OriginalSubtotal + cart.Delivery) - cart.Discount);
                cart.Disclaimer = true;
                cart.SubTotal = (cart.OriginalSubtotal + cart.Delivery + cart.Vat) - cart.Discount;
                cart.HasVoucher = false;

                return await _ctx.SaveChangesAsync() > 0;
            }
            else
            {
                decimal discountEligibleTotal = 0m;
                decimal noDiscountTotal = 0m;

                Domain.Models.Cart.Cart newCart = new Domain.Models.Cart.Cart();

                if (!string.IsNullOrEmpty(request.UserMark.userId))
                {
                    newCart.UserId = request.UserMark.userId;
                }
                else
                {
                    newCart.SessionId = request.UserMark.sessionId;
                }

                //var newCart = new Domain.Models.Cart.Cart
                //{
                //    UserId = request.UserMark.userId,
                //    SessionId = request.UserMark.sessionId,
                //};

                var newProduct = _ctx.Products.Find(request.ProductId);
                newCart.Products.Add(new CartProduct
                {
                    ProductId = request.ProductId,
                    Qty = request.Qty,
                    Product = newProduct
                });

                newCart.ProductCount = newCart.Products.Sum(x => x.Qty);
                newCart.OriginalSubtotal = Convert.ToDecimal(newCart.Products.Sum(x => x.Qty * x.Product.Price));
                newCart.Delivery = 60;
                if (newCart.OriginalSubtotal >= 650)
                {
                    newCart.Delivery = 0;
                }
                newCart.Discount = 0;

                //threshold discounts
                foreach (var p in newCart.Products)
                {
                    // if (p.Product.NoDiscount != true)
                    // {
                    discountEligibleTotal += ((p.Product.Price ?? 0m) * p.Qty);

                    if (p.Product.NoDiscount == true)
                    {
                        noDiscountTotal += ((p.Product.Price ?? 0m) * p.Qty);
                    }
                    // }
                }

                if (discountEligibleTotal >= 1500 && discountEligibleTotal < 2500)
                {
                    //apply 2.5% discounts
                    newCart.DiscountThreshold = 2.5m;
                    newCart.Discount = (discountEligibleTotal - noDiscountTotal) * 0.025m;
                }
                else if (newCart.OriginalSubtotal >= 2500)
                {
                    //apply 5% discounts
                    newCart.DiscountThreshold = 5m;
                    newCart.Discount = (discountEligibleTotal - noDiscountTotal) * 0.05m;
                }
                if (newCart.Discount > 0)
                {
                    newCart.Discounted = true;
                }

                newCart.Vat = CalculateVat((newCart.OriginalSubtotal + newCart.Delivery) - newCart.Discount);
                newCart.Disclaimer = true;
                newCart.SubTotal = (newCart.OriginalSubtotal + newCart.Delivery + newCart.Vat) - newCart.Discount; //minus discounts here
                newCart.HasVoucher = false;
                _ctx.Carts.Add(newCart);
                //_discounts.Value.CartTotal = newCart.Products.Sum(x => x.Qty * x.Product.Price.GetValueOrDefault(0m));
                return await _ctx.SaveChangesAsync() > 0;
            }
        }

        public decimal CalculateVat(decimal value)
        {
            return (value / 100) * 15;
        }
    }
}