using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Users;
using System.Collections.Generic;

namespace Shop.Domain.Models.Cart
{
    public class Cart
    {
        public Cart()
        {
            Products = new List<CartProduct>();
        }

        public int Id { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string OrderReference { get; set; }
        public ApplicationUser User { get; set; }

        public int? AddressId { get; set; }
        public Address Address { get; set; }

        public int? CostCenterId { get; set; }
        public CostCenter CostCenter { get; set; }

        public virtual ICollection<CartProduct> Products { get; set; }

        public decimal DiscountThreshold { get; set; }
        public bool Discounted { get; set; }

        public int ProductCount { get; set; }
        public int Delivery { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }
        public decimal OriginalSubtotal { get; set; }
        public bool Disclaimer { get; set; }
        public bool HasVoucher { get; set; }
        public string VoucherCode { get; set; }
    }
}
