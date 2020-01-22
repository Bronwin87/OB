using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain.Models.Products
{
    public class SubCategory
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public bool Deleted { get; set; }
        public string MainCategoryId { get; set; }
        public MainCategory MainCategory { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<TertiaryCategory> TertiaryCategories { get; set; }

    }
}
