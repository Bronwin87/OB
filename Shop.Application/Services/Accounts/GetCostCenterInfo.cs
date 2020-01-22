using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.EmailTemplates;
using Shop.Domain.Models.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;


namespace Shop.Application.Services.Accounts
{   
    public class GetCostCenterInfo
    {
        private ApplicationDbContext _ctx;

        public GetCostCenterInfo(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
    }

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

    }
}
