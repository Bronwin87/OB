using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Shop.Domain.Models.Products
{
    public class TertiaryCategory
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }

        public string SubCategoryId { get; set; }
        public bool Deleted { get; set; }
        public SubCategory SubCategory { get; set; }

        public ICollection<Product> Products { get; set; }

    }
}
