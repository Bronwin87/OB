using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.Products
{
    public class GetProductSearchQuery
    {
        public string SearchString { get; set; }
        public string CategoryId { get; set; }
        public string CategoryType { get; set; }
        public string UserId { get; set; }
        public bool ValueAddedProduct { get; set; }
    }

    public class GetProductSearch
    {
        private ApplicationDbContext _ctx;

        public GetProductSearch(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public IEnumerable<ProductViewModel> Do(GetProductSearchQuery query)
        {
            var discounts = _ctx.AccountUsers
                .Include(x => x.Account)
                .ThenInclude(x => x.Discounts)
                .FirstOrDefault(x => x.UserId == query.UserId && x.Active)?
                .Account?
                .Discounts;

            var realProducts = _ctx.Products.Include(x => x.TertiaryCategory).Where(x => x.Published != false);

            var actualRealProducts = FilterProduct(realProducts, query).ToList();

            var products = actualRealProducts.Select(x => new ProductViewModel
            {
                Id = x.Id,
                Name = x.Name,
                Description = x.Description,
                Price = GetPrice(x, null),
                PriceDec = x.Price ?? 0m,
                Discount = discounts?.Any(y => y.ProductId == x.Id) ?? false,
                DiscountedValue = GetPrice(x, discounts),
                ImageUrl = x.ImageUrl,
                UOM = x.Unit,
                Colour = x.Colour,
                Code = x.ExternalId,
                TertiaryCategory = x.TertiaryCategory.Name,
                TertiaryCategoryId = x.TertiaryCategoryId,
                ValueAdded =  x.ValueAddedProduct,
                OutOfStock = x.OutOfStock,
                brand = x.Brand
                

            }).OrderBy(x => x.Name);

            return products;
        }

        private IQueryable<Product> FilterProduct(IQueryable<Product> products, GetProductSearchQuery query)
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
                products = products.Where(x => EF.Functions.Like(x.SearchString, $"%{query.SearchString}%"));

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

        public class ProductViewModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Price { get; set; }
            public decimal PriceDec { get; set; }
            public string DiscountedValue { get; set; }
            public string ImageUrl { get; set; }
            public bool Discount { get; set; }
            public string UOM { get; set; }
            public string Colour { get; set; }
            public string Code { get; set; }
            public bool? Published { get; set; }
            public string brand { get; set; }
            public string TertiaryCategory { get; set; }
            public string TertiaryCategoryId { get; set; }
            public bool ValueAdded { get; set; }

            public string ProductAddPartialHtml { get; set; }
            public bool OutOfStock { get; set; }
        }
    }
}
