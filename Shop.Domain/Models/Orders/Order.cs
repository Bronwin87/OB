using Shop.Domain.Enums;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Users;
using System;
using System.Collections.Generic;

namespace Shop.Domain.Models.Orders
{
    public class Order
    {
        public Order()
        {
            OrderProducts = new List<OrderProduct>();
        }

        public int Id { get; set; }

        public string OrderNumber { get; set; }
        public string OrderRef { get; set; } 
        public string PaymentReference { get; set; } 

        public DateTime Created { get; set; }

        public OrderStatus Status { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string AuthorizerId { get; set; }
        //public ApplicationUser Authorizer { get; set; }
        public DateTime? AuthorizedDate { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public ICollection<OrderProduct> OrderProducts { get; set; }

        public int? LocationId { get; set; }
        public Location Location { get; set; }
        public int? CostCenterId { get; set; }
        public CostCenter CostCenter { get; set; }

        public int ProductCount { get; set; }
        public int Delivery { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Discount { get; set; }
        public decimal OriginalSubtotal { get; set; }
        public bool Disclaimer { get; set; }
        public bool HasVoucher { get; set; }
        public string VoucherCode { get; set; }
        public int? Cin7ID { get; set; }
    }
}
