using Microsoft.AspNetCore.Identity;
using Shop.Domain.Enums;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Orders;
using Shop.Domain.Models.PendingOrders;
using Shop.Domain.Models.Quotes;
using System.Collections.Generic;

namespace Shop.Domain.Models.Users
{
    public class ApplicationUser : IdentityUser
    {
        public ApplicationUser()
        {
            Accounts = new List<AccountUser>();
            Orders = new List<Order>();
            Addresses = new List<Address>();
            AssignedUsers = new List<ApplicationUser>();
        }

        public string FirstName { get; set; } = "undefined";
        public string LastName { get; set; } = "undefined";

        public UserType Type { get; set; }

        public virtual IEnumerable<AccountUser> Accounts { get; set; }
        public virtual ICollection<Address> Addresses { get; set; }

        public string JobTitle { get; set; } = "undefined";

        public ICollection<Order> Orders { get; set; }

        public ICollection<PendingOrder> PendingOrders { get; set; }

        public ICollection<Quote> Quotes { get; set; }

        public ICollection<ApplicationUser> AssignedUsers { get; set; }

        public string AuthorizerId { get; set; }

        public ApplicationUser Authorizer { get; set; }
    }
}
