using Shop.Domain.Enums;
using Shop.Domain.Models.Accounts;

namespace Shop.Domain.Models.Products
{
    public class Discount
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int DiscountPercent { get; set; }
        public int DiscountValue { get; set; }
        public DiscountType DiscountType { get; set; }
    }
}
