using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.Orders
{
    public class OrderProduct
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int Qty { get; set; }
    }
}
