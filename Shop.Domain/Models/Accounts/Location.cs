using Shop.Domain.Models.Orders;
using Shop.Domain.Models.Users;
using System.Collections.Generic;

namespace Shop.Domain.Models.Accounts
{
    public class Location
    {
        public Location()
        {
            CostCenters = new List<CostCenter>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = "undefined";
        public string PhoneNumber { get; set; } = "undefined";

        public int AccountId { get; set; }
        public Account Account { get; set; }

        public int AddressId { get; set; }
        public Address Address { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string AuthorizerId { get; set; }
        public ApplicationUser Authorizer { get; set; }

        public virtual ICollection<CostCenter> CostCenters { get; set; }

        public virtual ICollection<LocationAuth> LocationAuths { get; set; }
        //public virtual ICollection<Order> Orders { get; set; }
    }

    public partial class LocationAuth
    {
        public int Id { get; set; }
        public string AuthorizerId { get; set; } 
        public string Userid { get; set; }
        public int LocationId { get; set; }

        public virtual ApplicationUser User { get; set; }
    }

}
