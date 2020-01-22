using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain.Models.Products
{
    public class Product
    {
        [Key]
        public string Id { get; set; }
        public bool Published { get; set; }
        public bool OutOfStock { get; set; }
        public string BarCode { get; set; }
        public string Brand { get; set; }
        public string Name { get; set; }
        public string SearchString { get; set; }
        public decimal? Price { get; set; }
        public decimal CostPrice { get; set; }
        public string ImageUrl { get; set; }
        public bool? ImageUpdated { get; set; }

        public string MainCategoryId { get; set; }
        public MainCategory MainCategory { get; set; }

        public string SubCategoryId { get; set; }
        public SubCategory SubCategory { get; set; }

        public string TertiaryCategoryId { get; set; }
        public TertiaryCategory TertiaryCategory { get; set; }

        public string ExternalId { get; set; }
        public string Unit { get; set; }
        public string Colour { get; set; }
        public string Description { get; set; }
        public bool? NoDiscount { get; set; }
        public bool ValueAddedProduct { get; set; }

        public virtual ICollection<Discount> Discounts { get; set; }
        public virtual ICollection<ProductLink> Links { get; set; }
    }
}
