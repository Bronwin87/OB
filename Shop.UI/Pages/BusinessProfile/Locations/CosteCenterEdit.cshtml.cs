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
using Microsoft.Extensions.Options;
using Shop.Application.Cart;
using Shop.Application.Products;
using Shop.Application.Services.FavouriteLists;
using Shop.Domain.Models;
using Shop.Domain.Models.Products;
using Shop.UI.Infrastructure;

namespace Shop.UI.Pages.BusinessProfile.Locations
{
    public class CosteCenterEditModel : PageModel
    {
        public ApplicationDbContext _ctx;
        public CosteCenterEditModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public bool CanEdit { get; private set; }

        [BindProperty]
        public CostCenterViewModel Input { get; set; }

        public LocationViewModel Location { get; set; }
        public IEnumerable<LocationViewModel> Locations { get; set; }

        public Account Account { get; set; }
        public CostCenterViewModel CostCenter { get; set; }

        public int CostCenterId { get; set; }
        public int LocationId { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }
        public List<UserViewModel> Authorizers { get; set; }

        public void OnGet(int costCenterID, int locationId)
        {
            Input = new CostCenterViewModel();
            CostCenterId = costCenterID;
            LocationId = locationId;

            CanEdit = User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "authorizer"
               || User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "superuser";

            //////////////////////
            Account = _ctx.AccountUsers
                   //.Include(x => x.Account)
                   //    .ThenInclude(x => x.Locations)
                   //    .ThenInclude(x => x.Address)
                   //.Include(x => x.Account)
                   //    .ThenInclude(x => x.Locations)
                   //    .ThenInclude(x => x.CostCenters)
                   //    .ThenInclude(x => x.CostCenterAuth)
                   .Include(x => x.Account)
                       .ThenInclude(x => x.AccountUsers)
                       .ThenInclude(x => x.User)
                   .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                   .Account;

            var costCenter = _ctx.CostCenters
                                 .Include(c => c.CostCenterAuth)
                                 .Include(c => c.Location)
                                 .FirstOrDefault(c => c.Id == costCenterID);
            var costCenterUsers = new List<CostCenterAuth>();
            if (costCenter != null)
            {
                CostCenter = new CostCenterViewModel()
                {
                    Id = costCenterID,
                    Name = costCenter.Name,
                    LocationId = costCenter.LocationId,
                    Location = costCenter.Location.Name,
                    AuthorizerId = costCenter.AuthorizerId,
                    //UserId = costCenter.UserId,
                };

                costCenterUsers = costCenter.CostCenterAuth.ToList();
            }
            else
            {
                CostCenter = new CostCenterViewModel();
            }

            Users = Account.AccountUsers.Where(x => x.User.Type == Domain.Enums.UserType.BusinessUser).Select(x => x.User).OrderBy(s => s.FirstName).ToList();
            Authorizers = Account
                               .AccountUsers
                               .Where(x => x.User.Type == Domain.Enums.UserType.Authorizer)
                               .Select(u => new UserViewModel()
                               {
                                   Id = u.User.Id,
                                   Email = u.User.Email,
                                   Name = u.User.FirstName,
                                   Surname = u.User.LastName
                               })
                               .OrderBy(u => u.Name)
                               .ToList();

            foreach (var item in Users)
            {
                Input.Users.Add(new UserViewModel()
                {
                    Id = item.Id,
                    Email = item.Email,
                    Name = item.FirstName,
                    Surname = item.LastName,
                    IsChecked = costCenterUsers.Any(ca => ca.Userid == item.Id)
                });
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var costCenter = new CostCenter();

            if (Input.Id > 0)
            {
                costCenter = _ctx.CostCenters
                                 .Include(c => c.CostCenterAuth)
                                 .FirstOrDefault(c => c.Id == Input.Id);

                foreach (var item in Input.Users)
                {
                    if (item.IsChecked)
                    {
                        if (!costCenter.CostCenterAuth.Any(la => la.Userid == item.Id))
                        {
                            costCenter.CostCenterAuth.Add(new CostCenterAuth()
                            {
                                Userid = item.Id
                            });
                        }
                    }
                    else
                    {
                        var locationAuth = costCenter.CostCenterAuth.FirstOrDefault(la => la.Userid == item.Id);
                        if (locationAuth != null)
                        {
                            costCenter.CostCenterAuth.Remove(locationAuth);
                        }
                    }
                }
            }
            else
            {
                costCenter = new CostCenter()
                {
                    LocationId = Input.LocationId,
                    CostCenterAuth = new List<CostCenterAuth>()
                };

                foreach (var item in Input.Users)
                {
                    if (item.IsChecked)
                    {
                        costCenter.CostCenterAuth.Add(new CostCenterAuth()
                        {
                            Userid = item.Id
                        });
                    }
                }

                _ctx.CostCenters.Add(costCenter);
            }

            costCenter.Name = Input.Name;
            costCenter.AuthorizerId = Input.AuthorizerId;
            await _ctx.SaveChangesAsync();

            return RedirectToPage("/BusinessProfile/Locations/LocationEdit", new { locationId = Input.LocationId });
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
            public IEnumerable<CostCenterViewModel> CostCenters { get; set; }
        }
        public class CostCenterViewModel
        {
            public CostCenterViewModel()
            {
                this.Users = new List<UserViewModel>();
            }

            public int Id { get; set; }
            public int LocationId { get; set; }
            public string Location { get; set; }
            public string Name { get; set; }
            public string UserId { get; set; }
            public string AuthorizerId { get; set; }

            public List<UserViewModel> Users { get; set; }
        }

        public class UserViewModel
        {
            public string Id { get; set; }
            public bool IsChecked { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string Surname { get; set; }
            public string DisplayName
            {
                get
                {
                    return this.Name + " " + this.Surname;
                }
            }
        }
    }
}