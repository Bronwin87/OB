using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System.Linq;
using Shop.Application.Services.Cart;
using Shop.Application.Cart;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;
using System;
using Shop.Application.Services.Products.Entities;

namespace Shop.Application.Products
{
    public class GetVAProductsQuery
    {
        public string SearchString { get; set; }
        public string CategoryId { get; set; }
        public string CategoryType { get; set; }
        public string UserId { get; set; }
        public bool ValueAddedProduct { get; set; }
    }

    public class GetVAProducts
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public GetVAProducts(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _discounts = discounts;
        }

        public IEnumerable<ProductViewModel> Do(GetVAProductsQuery query)
        {
            var discounts = _ctx.AccountUsers
                .Include(x => x.Account)
                .ThenInclude(x => x.Discounts)
                .FirstOrDefault(x => x.UserId == query.UserId && x.Active)?
                .Account?
                .Discounts;

            var realProducts = _ctx.Products.Where(x => x.Published != false).Where(y => y.ValueAddedProduct == true);
            var actualRealProducts = FilterProduct(realProducts, query).ToList();


            if (_discounts.Value.DiscountPercentage > 0)
            {
                var products = actualRealProducts.Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = GetPrice(x, null),
                    //discounts for dynamic pricing
                    Discount = discounts?.Any(y => y.ProductId == x.Id) ?? false,
                    DiscountedValue = GetPrice(x, discounts),
                    ImageUrl = x.ImageUrl,
                    UOM = x.Unit,
                    Colour = x.Colour,
                    Code = x.ExternalId  ,
                    ValueAdded = x.ValueAddedProduct
                });
                
                foreach (var x in products)
                {
                    if (x.Discount != true)
                    {
                       // x.ThresholdDiscountedPrice = Convert.ToDecimal(x.Price) * _discounts.Value.DiscountPercentage;
                    }
                }
                return products;
            }
            else
            {
                var products = actualRealProducts.Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    Price = GetPrice(x, null),
                    //discounts for dynamic pricing
                    Discount = discounts?.Any(y => y.ProductId == x.Id) ?? false,
                    DiscountedValue = GetPrice(x, discounts),
                    ImageUrl = x.ImageUrl,
                    UOM = x.Unit,
                    Colour = x.Colour,
                    Code = x.ExternalId,
                    ValueAdded = x.ValueAddedProduct,
                    OutOfStock = x.OutOfStock

                });

                
                return products;
            }

            //return products;
        }

        private IQueryable<Product> FilterProduct(IQueryable<Product> products, GetVAProductsQuery query)
        {
            if (!string.IsNullOrEmpty(query.CategoryId))
            {
                if (query.CategoryType == "main")
                    products = products.Where(x => x.MainCategoryId == query.CategoryId);
                else if (query.CategoryType == "sub")
                    products = products.Where(x => x.SubCategoryId == query.CategoryId);
                else if (query.CategoryType == "tri")
                    products = products.Where(x => x.TertiaryCategoryId == query.CategoryId);
            }

            if (!string.IsNullOrEmpty(query.SearchString))
                products = products.Where(x => EF.Functions.Like(x.Name, $"%{query.SearchString}%"));

            return products;
        }

        private string GetPrice(Product product, ICollection<Discount> discounts)
        {
            if (!product.Price.HasValue)
                return "No Price";

            if (discounts == null)
                return $"R{product.Price.Value.ToString("N2")}";

            var discount = discounts.FirstOrDefault(x => x.ProductId == product.Id);

            if (discount == null)
                return $"R{product.Price.Value.ToString("N2")}";

            if (discount.DiscountType == Domain.Enums.DiscountType.Percent)
                return $"R{(product.Price.Value * discount.DiscountPercent / 100).ToString("N2")}";

            if (discount.DiscountType == Domain.Enums.DiscountType.Value)
                return $"R{(product.Price.Value - discount.DiscountValue).ToString("N2")}";

            return "ERROR";
        }
    }
}
