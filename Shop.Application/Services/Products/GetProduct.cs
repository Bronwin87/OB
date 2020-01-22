using Shop.Database;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;

namespace Shop.Application.Products
{
    public class GetProduct
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public GetProduct(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        public IEnumerable<ProductLink> RelatedProductLinks { get; set; }
        public IEnumerable<Shop.Application.Services.Products.Entities.ProductViewModel> RelatedProducts { get; set; }

        public async Task<ProductViewModel> Do(string id)
        {

            return _ctx.Products
                //.Include(x => x.Stock)
                .Where(x => x.Id == id)
                .Select(x => new ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    //Value = $"R{x.Value.ToString("N2")}", // 1100.50 => 1,100.50 => R 1,100.50
                    ImageUrl = x.ImageUrl,
                    Brand = x.Brand,
                    ExternalId = x.ExternalId,
                    Unit = x.Unit,
                    Colour = x.Colour,
                    ProductInfo = x.Description,
                    Value = x.Price,
                    ValueAdded = x.ValueAddedProduct,
                    //
                    MainCategoryId = x.MainCategoryId,
                    MainCategory = x.MainCategory.CamelName,
                    SubCategoryId = x.SubCategoryId,
                    SubCategory = x.SubCategory.Name,
                    TertiaryCategoryId = x.TertiaryCategoryId,
                    TertiaryCategory = x.TertiaryCategory.Name,
                    outOfStock = x.OutOfStock
                })
                .FirstOrDefault();
        }

        public decimal GetDiscouts(IEnumerable<Product> products)
        {
            Cart.GetCart gc = new Cart.GetCart(_ctx, _discounts);
            return 1;
        }

        public IEnumerable<Shop.Application.Services.Products.Entities.ProductViewModel> GetRelatedProducts(string productID)
        {
            List<Shop.Application.Services.Products.Entities.ProductViewModel> pList = new List<Shop.Application.Services.Products.Entities.ProductViewModel>();
            RelatedProductLinks = _ctx.ProductLinks.Where(x => x.RootId == productID);
            foreach (var link in RelatedProductLinks)
            {
                var product = _ctx.Products
                //.Include(x => x.Stock)
                .Where(x => x.Id == link.TargetId)
                .Select(x => new Shop.Application.Services.Products.Entities.ProductViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    //Value = $"R{x.Value.ToString("N2")}", // 1100.50 => 1,100.50 => R 1,100.50
                    ImageUrl = x.ImageUrl,
                    Code = x.ExternalId,
                    UOM = x.Unit,
                    Colour = x.Colour,
                    Price = x.Price.ToString(),
                    ValueAdded = x.ValueAddedProduct,
                    Published = x.Published,                   
                    //

                    OutOfStock = x.OutOfStock
                })
                .FirstOrDefault();
                pList.Add(product);
            }

            RelatedProducts = pList;
            return RelatedProducts;
         
        }

        public class ProductViewModel
        {
            public string Id { get; set; }
            public string Brand { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal? Value { get; set; }
            public string ImageUrl { get; set; }
            public string ExternalId { get; set; }
            public string Unit { get; set; }
            public string Colour { get; set; }
            public string ProductInfo { get; set; }
            public bool ValueAdded { get; set; }
            public bool outOfStock { get; set; }
            // to be added to domain model non existent in the database

            public string MainCategoryId { get; set; }
            public string MainCategory { get; set; }
            public string SubCategoryId { get; set; }
            public string SubCategory { get; set; }
            public string TertiaryCategoryId { get; set; }
            public string TertiaryCategory { get; set; }

            public string Keywords
            {
                get
                {
                    return string.Format("Stationery, online shopping, ecommerce, {0}, {1}, {2}, {3}, {4}", MainCategory, SubCategory, TertiaryCategory, Brand, Colour);
                }
            }
        }
    }
}
