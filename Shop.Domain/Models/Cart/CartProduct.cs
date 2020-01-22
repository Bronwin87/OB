using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.Cart
{
    public class CartProduct
    {
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int Qty { get; set; }

        //public decimal CostPrice { get; set; }

        //public decimal OriginalPrice { get; set; }
        //public decimal OriginalSubtotal { get; set; }

        //public decimal Discount { get; set; }

        //public decimal FinalPrice { get; set; }
        //public decimal FinalSubtotal { get; set; }
    }
}
