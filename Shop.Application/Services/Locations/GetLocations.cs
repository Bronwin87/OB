using System;
using System.Collections.Generic;
using System.Text;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Users;

namespace Shop.Application.Services.Locations
{
    public class GetLocations
    {
        private ApplicationDbContext _ctx;
        public GetLocations(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }


        public class Response
        {
            public int Id { get; set; }
            public string CompanyName { get; set; }
            public string PhoneNumber { get; set; }

            public int AccountId { get; set; }
            public Account Account { get; set; }

            public int AddressId { get; set; }
            public Address Address { get; set; }

            public string UserId { get; set; }
            public ApplicationUser User { get; set; }

            public string AuthorizerId { get; set; }
            public ApplicationUser Authorizer { get; set; }

            public virtual ICollection<CostCenter> CostCenters { get; set; }
        }

    
    }
}
