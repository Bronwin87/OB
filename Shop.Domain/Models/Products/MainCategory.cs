using System.Collections.Generic;

namespace Shop.Domain.Models.Products
{
    public class MainCategory
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string CamelName { get; set; }
        public int Order { get; set; }
        public bool Deleted { get; set; }

        public ICollection<Product> Products { get; set; }
        public ICollection<SubCategory> SubCategories { get; set; }
    }
}
