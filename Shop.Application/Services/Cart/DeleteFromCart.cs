using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Cart;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Cart
{
    public class DeleteFromCart
    {
        private ApplicationDbContext _ctx;

        public DeleteFromCart(ApplicationDbContext ctx)
        {
            _ctx = ctx;
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


            CartProduct cp = cart.Products.FirstOrDefault(p => p.ProductId == request.ProductId);
            if (cp != null)
            {
                cart.Products.Remove(cp);
            }

            var delete = await _ctx.SaveChangesAsync() > 0;

            if (delete)
            {
                decimal discountEligibleTotal = 0m;
                cart.Discounted = false;
                cart.Discount = 0;
                cart.Delivery = 60;
                cart.DiscountThreshold = 0;

                var exitingProduct = cart.Products.FirstOrDefault(x => x.ProductId == request.ProductId);

                //if (exitingProduct != null)
                //{
                //    exitingProduct.Qty = request.Qty;
                //}
                //else
                //{
                //    var newProduct = _ctx.Products.Find(request.ProductId);
                //    cart.Products.Add(new CartProduct
                //    {
                //        ProductId = request.ProductId,
                //        Qty = request.Qty,
                //        Product = newProduct
                //    });
                //}

                cart.ProductCount = cart.Products.Sum(x => x.Qty);
                cart.OriginalSubtotal = Convert.ToDecimal(cart.Products.Sum(x => x.Qty * x.Product.Price));

                if (cart.OriginalSubtotal >= 650)
                {
                    cart.Delivery = 0;
                }

                //threshold discounts
                foreach (var p in cart.Products)
                {
                    if (p.Product.NoDiscount != true)
                    {
                        discountEligibleTotal += ((p.Product.Price ?? 0m) * p.Qty);
                    }
                }

                if (discountEligibleTotal >= 1500 && discountEligibleTotal < 2500)
                {
                    //apply 2.5% discounts
                    cart.DiscountThreshold = 2.5m;
                    cart.Discount = discountEligibleTotal * 0.025m;
                }
                else if (cart.OriginalSubtotal >= 2500)
                {
                    //apply 5% discounts
                    cart.DiscountThreshold = 5m;
                    cart.Discount = discountEligibleTotal * 0.05m;
                }

                if (cart.Discount > 0)
                {
                    cart.Discounted = true;
                }

                cart.Vat = CalculateVat((cart.OriginalSubtotal + cart.Delivery) - cart.Discount);
                cart.SubTotal = (cart.OriginalSubtotal + cart.Delivery + cart.Vat) - cart.Discount;

               
            }


    
            return await _ctx.SaveChangesAsync() > 0;


        }

        public async Task<bool> DoClear(Request request)
        {
            var cart = _ctx.Carts
                .Include(x => x.Products)
                .ThenInclude(x => x.Product)
                .FirstOrDefault(x => (x.UserId == request.UserMark.userId
                        && !string.IsNullOrEmpty(request.UserMark.userId))
                    || x.SessionId == request.UserMark.sessionId);

            foreach(var cartProduct in cart.Products.ToList())
            {
                cart.Products.Remove(cartProduct);
            }

            return await _ctx.SaveChangesAsync() > 0;
        }
        public decimal CalculateVat(decimal value)
        {
            return (value / 100) * 15;
        }
    }
}
