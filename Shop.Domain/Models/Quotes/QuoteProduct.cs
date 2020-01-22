using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.Quotes
{
    public class QuoteProduct
    {
        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int Qty { get; set; }

        public int QuoteId { get; set; }
        public Quote Quote { get; set; }
    }
}
