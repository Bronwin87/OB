using System.Collections.Generic;

namespace Shop.Application.Services.ProductsAdmin.Entities
{
    public class ProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Value { get; set; }
        public bool Published { get; set; }
        public bool OutOfStock { get; set; }
        public bool NoDiscount { get; set; }
        public bool ValueAdd { get; set; }
        public string SearchString { get; set; }
        public decimal CostPrice { get; set; }
        public string ImageUrl { get; set; }
        public string Main { get; set; }
        public string Sub { get; set; }
        public string Tri { get; set; }
        public string ExternalId { get; set; }
        public string Unit { get; set; }
        public string Colour { get; set; }
        public string Brand { get; set; }

        public IEnumerable<ProductViewModel> Linked { get; set; }
        public IEnumerable<ProductViewModel> Alternative { get; set; }
        public IEnumerable<ProductViewModel> ValueAdded { get; set; }
    }
}
