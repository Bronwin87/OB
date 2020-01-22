namespace Shop.Application.Services.Products.Entities
{
    public class ProductViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Price { get; set; }
        public string DiscountedValue { get; set; }
        public string ImageUrl { get; set; }
        public bool Discount { get; set; }
        public string UOM { get; set; }
        public string Colour { get; set; }
        public string Code { get; set; }
        public bool? Published { get; set; }
        public decimal ThresholdDiscountedPrice { get; set; }
        public bool ValueAdded { get; set; }
        public bool OutOfStock { get; set; }
    }
}

