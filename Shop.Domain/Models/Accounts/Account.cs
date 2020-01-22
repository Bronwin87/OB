using Shop.Domain.Models.Orders;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using Shop.Domain.Models.PendingOrders;
using Shop.Domain.Models.Quotes;

namespace Shop.Domain.Models.Accounts
{
    public class Account
    {
        public Account()
        {
            References = new List<Reference>();
            Locations = new List<Location>();
            AccountUsers = new List<AccountUser>();
            Orders = new List<Order>();
            Discounts = new List<Discount>();
        }
        public int Id { get; set; } 
        public string CompanyName { get; set; } = "";
        public string RegistrationNumber { get; set; } = "";
        public string VatNumber { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        public string Email { get; set; } = "";
        public int AddressId { get; set; } 
        public Address Address { get; set; }
        public virtual ICollection<Reference> References { get; set; }
        public virtual ICollection<Location> Locations { get; set; }
        public virtual ICollection<AccountUser> AccountUsers { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public bool TermAccount { get; set; }
        public bool ThirtyDayTermApproved { get; set; }
        public virtual ICollection<Discount> Discounts { get; set; }
        public ICollection<PendingOrder> PendingOrders { get; set; }
        public ICollection<Quote> Quotes { get; set; }
        public double Limit { get; set; }
    }
}