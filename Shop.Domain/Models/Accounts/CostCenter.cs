using Shop.Domain.Models.Orders;
using Shop.Domain.Models.Users;
using System.Collections.Generic;

namespace Shop.Domain.Models.Accounts
{
    public class CostCenter
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int LocationId { get; set; }
        public Location Location { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string AuthorizerId { get; set; }
        public ApplicationUser Authorizer { get; set; }

        public virtual ICollection<CostCenterAuth> CostCenterAuth { get; set; }
        //public virtual ICollection<Order> Orders { get; set; }
    }   

    public partial class CostCenterAuth
    {
        public int Id { get; set; }
        public string AuthorizerId { get; set; }
        public string Userid { get; set; }
        public int CostCenterId { get; set; }

        public ApplicationUser User { get; set; }
    }
}
