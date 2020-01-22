using System;
using System.Collections.Generic;
using System.Text;
using Shop.Database;
using Shop.Domain;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.Cart;
using System.Linq;

namespace Shop.Application.Services.Cart
{
    public class DiscountThresholdService
    {
        List<string> excludedCodes = new List<string>
        {
           "3PR4do1ZJJ","JjmyxWghnZ","wm3p8cqY9H"
        };

        private ApplicationDbContext _ctx;
        public DiscountThresholdService(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public Domain.Models.Cart.Cart Do(Domain.Models.Cart.Cart cart)
        {
            if (!cart.Discounted && cart.DiscountThreshold == 5m)
            {                
                decimal? totalAmount = cart.Products.Sum(x => x.Qty * x.Product.Price);

                if (totalAmount >= 1500)
                {
                    cart.DiscountThreshold = 2.5m;
                    //apply 5% discount
                    if (totalAmount >= 2500)
                    {
                        cart.DiscountThreshold = 5m;
                        foreach (var x in cart.Products)
                        {
                            if (!excludedCodes.Contains(x.Product.SubCategoryId))
                            {
                                x.Product.Price = x.Product.Price - (x.Product.Price * cart.DiscountThreshold);
                            }
                            cart.Discounted = true;
                            return cart;
                        }
                    }
                    //apply 2.5% discount
                    foreach (var x in cart.Products)
                    {
                        if (!excludedCodes.Contains(x.Product.SubCategoryId))
                        {
                            x.Product.Price = x.Product.Price - (x.Product.Price * cart.DiscountThreshold);
                        }
                        cart.Discounted = true;
                        return cart;
                    }                    
                }
                return cart;
            }
            return cart;
        }
    }
}
