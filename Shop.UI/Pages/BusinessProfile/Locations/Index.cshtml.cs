using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Users;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile.Locations
{

    public class IndexModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public IndexModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [BindProperty]
        public LocationViewModel Input { get; set; }

        [BindProperty]
        public ConstCenterViewModel Input2 { get; set; }

        public string Message { get; set; }
        public bool CanEdit { get; private set; }
        public IEnumerable<LocationViewModel> Locations { get; set; }
        public IEnumerable<Location> UserLocations { get; set; }

        public Account Account { get; set; }
        public IEnumerable<ApplicationUser> Users { get; set; }
        public IEnumerable<ApplicationUser> Authorizers { get; set; }

        public void OnGet(string message)
        {
            Message = message;
            CanEdit = User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "authorizer"
                || User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "superuser";

            //to view multiple locations for this account
            Account = _ctx.AccountUsers
                .Include(x => x.Account)
                    .ThenInclude(x => x.Locations)
                    .ThenInclude(x => x.Address)
                .Include(x => x.Account)
                    .ThenInclude(x => x.Locations)
                    .ThenInclude(x => x.CostCenters)
                    .ThenInclude(x => x.User)
                    .ThenInclude(x => x.Authorizer)
                .Include(x => x.Account)
                    .ThenInclude(x => x.AccountUsers)
                    .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                .Account;

            Locations = Account.Locations
                .Select(x => new LocationViewModel
                {
                    Id = x.Id,
                    CompanyName = x.Name,
                    Address1 = x.Address.Address1,
                    Address2 = x.Address.Address2,
                    City = x.Address.City,
                    PostCode = x.Address.PostCode,
                    PhoneNumber = x.PhoneNumber,
                    UserId = x.UserId,
                    AuthorizerId = x.AuthorizerId,
                    CostCenters = x.CostCenters.Select(y => new ConstCenterViewModel
                    {
                        Id = y.Id,
                        LocationId = x.Id,
                        Name = y.Name,
                        UserId = y.UserId,
                        UserEmail = y.User?.Email,
                        AuthorizerId = y.AuthorizerId,
                        AuthorizerEmail = y.Authorizer?.Email
                    })
                });
            Users = Account.AccountUsers.Where(x => x.User.Type != Domain.Enums.UserType.Authorizer).Select(x => x.User);
            Authorizers = Account.AccountUsers.Where(x => x.User.Type == Domain.Enums.UserType.Authorizer).Select(x => x.User);
        }

        public async Task<IActionResult> OnPost(string submitAction)
        {
            if (int.TryParse(submitAction, out int locationId))
            {
                var location = await _ctx.Locations
                                   .Include(l => l.CostCenters)
                                   .FirstOrDefaultAsync(l => l.Id == locationId);

                if(location != null)
                {
                    foreach(var item in location.CostCenters.ToList())
                    {
                        _ctx.CostCenters.Remove(item);
                    }
                    //foreach(var item in location.LocationAuths.ToList())
                    //{
                    //    // it cascade deletes?
                    //    location.LocationAuths.Remove(item);
                    //}

                    _ctx.Locations.Remove(location);
                    await _ctx.SaveChangesAsync();
                }
            }

            return RedirectToPage("/BusinessProfile/Locations/Index");
        }

        public class LocationViewModel
        {
            public int Id { get; set; }
            public int AccountId { get; set; }
            [Required]
            public string CompanyName { get; set; }
            [Required]
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            public string PostCode { get; set; }
            [Required]
            public string PhoneNumber { get; set; }
            public string UserId { get; set; }
            public string AuthorizerId { get; set; }
            public IEnumerable<ConstCenterViewModel> CostCenters { get; set; }
        }
        public class ConstCenterViewModel
        {
            public int Id { get; set; }
            public int LocationId { get; set; }
            public string Name { get; set; }
            public string UserId { get; set; }
            public string UserEmail { get; set; }
            public string AuthorizerId { get; set; }
            public string AuthorizerEmail { get; set; }
        }
    }
}