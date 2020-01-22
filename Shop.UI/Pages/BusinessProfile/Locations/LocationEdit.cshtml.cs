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
    public class LocationEditModel : PageModel
    {
        public ApplicationDbContext _ctx;
        public LocationEditModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        [BindProperty]
        public LocationViewModel Input { get; set; }

        [BindProperty]
        public CostCenterViewModel Input2 { get; set; }

        public bool CanEdit { get; private set; }
        public LocationViewModel Location { get; set; }
        public IEnumerable<LocationViewModel> Locations { get; set; }

        public IEnumerable<Location> UserLocations { get; set; }
        public int LocId { get; set; }
        public Account Account { get; set; }
        public List<ApplicationUser> Users { get; set; }
        public List<UserViewModel> Authorizers { get; set; }

        public void OnGet(int locationId)
        {
            Input = new LocationViewModel();
            LocId = locationId;

            CanEdit = User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "authorizer"
                || User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "superuser";

            Account = _ctx.AccountUsers
                    .Include(x => x.Account)
                        .ThenInclude(x => x.Locations)
                        .ThenInclude(x => x.Address)
                    .Include(x => x.Account)
                        .ThenInclude(x => x.Locations)
                        .ThenInclude(x => x.CostCenters)
                        .ThenInclude(x => x.CostCenterAuth)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Account)
                        .ThenInclude(x => x.Locations)
                        .ThenInclude(x => x.CostCenters)
                        .ThenInclude(x => x.Authorizer)
                    .Include(x => x.Account)
                        .ThenInclude(x => x.Locations)
                        .ThenInclude(x => x.LocationAuths)
                    .Include(x => x.Account)
                        .ThenInclude(x => x.AccountUsers)
                        .ThenInclude(x => x.User)
                    .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                    .Account;

            var location = Account.Locations.FirstOrDefault(l => l.Id == LocId);
            if (location != null)
            {
                Location = new LocationViewModel
                {
                    Id = location.Id,
                    CompanyName = location.Name,
                    Address1 = location.Address.Address1,
                    Address2 = location.Address.Address2,
                    City = location.Address.City,
                    PostCode = location.Address.PostCode,
                    PhoneNumber = location.PhoneNumber,
                    //UserId = x.UserId,
                    AuthorizerId = location.AuthorizerId,
                    CostCenters = location.CostCenters.Select(y => new CostCenterViewModel
                    {
                        Id = y.Id,
                        LocationId = location.Id,
                        Name = y.Name,
                        //UserId = y.UserId,
                        AuthorizerId = y.AuthorizerId,
                        AuthorizerEmail = y.Authorizer?.Email,
                        AuthorizerName = y.Authorizer?.FirstName,
                        AuthorizerSurname = y.Authorizer?.LastName,
                        Users = y.CostCenterAuth.Select(ca => new UserViewModel()
                        {
                            Email = ca.User?.Email,
                            Name = ca.User?.FirstName,
                            Surname = ca.User?.LastName
                        }).ToList()
                    })
                };

                //Input.AuthorizerId = Location.AuthorizerId;
            }
            else
            {
                Location = new LocationViewModel();
            }

            if (!Location.CostCenters.Any())
            {
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
                //.OrderBy(s => s.User.FirstName);

                if (location != null)
                {
                    var locationUsers = location.LocationAuths.ToList();
                    foreach (var item in Users)
                    {
                        Input.Users.Add(new UserViewModel()
                        {
                            Id = item.Id,
                            Email = item.Email,
                            IsChecked = locationUsers.Any(la => la.Userid == item.Id),
                            Name = item.FirstName,
                            Surname = item.LastName
                        });
                    }
                }
                else
                {
                    foreach (var item in Users)
                    {
                        Input.Users.Add(new UserViewModel()
                        {
                            Id = item.Id,
                            Email = item.Email,
                            Name = item.FirstName,
                            Surname = item.LastName
                        });
                    }
                }
            }
        }

        public async Task<IActionResult> OnPost()
        {
            var OldLocation = _ctx.Locations
                .Include(x => x.User)
                .Where(x => x.User != null && x.User.Type == Domain.Enums.UserType.BusinessUser)
                .FirstOrDefault(x => x.UserId == Input.UserId);

            if (OldLocation != null)
                OldLocation.UserId = null;

            if (Input.Id > 0)
            {
                var Location = _ctx.Locations
                                    .Include(x => x.Address)
                                    .Include(x => x.LocationAuths)
                                    .FirstOrDefault(x => x.Id == Input.Id);

                Location.Name = Input.CompanyName;
                Location.Address.Address1 = Input.Address1;
                Location.Address.Address2 = Input.Address2;
                Location.Address.City = Input.City;
                Location.Address.PostCode = Input.PostCode;
                Location.PhoneNumber = Input.PhoneNumber;
                Location.AuthorizerId = Input.AuthorizerId;

                foreach (var item in Input.Users)
                {
                    if (item.IsChecked)
                    {
                        if (!Location.LocationAuths.Any(la => la.Userid == item.Id))
                        {
                            Location.LocationAuths.Add(new LocationAuth()
                            {
                                Userid = item.Id
                            });
                        }
                    }
                    else
                    {
                        var locationAuth = Location.LocationAuths.FirstOrDefault(la => la.Userid == item.Id);
                        if (locationAuth != null)
                        {
                            Location.LocationAuths.Remove(locationAuth);
                        }
                    }
                }
            }
            else
            {
                Account = _ctx.Accounts.FirstOrDefault(x => x.Id == Input.AccountId);

                var newLocation = new Location
                {
                    Name = Input.CompanyName,
                    PhoneNumber = Input.PhoneNumber,
                    AuthorizerId = Input.AuthorizerId,
                    Address = new Address
                    {
                        Address1 = Input.Address1,
                        Address2 = Input.Address2,
                        City = Input.City,
                        PostCode = Input.PostCode,
                        Country = "SOUTH AFRICA"
                    },
                    LocationAuths = new List<LocationAuth>()
                };

                foreach (var item in Input.Users)
                {
                    if (item.IsChecked)
                    {
                        newLocation.LocationAuths.Add(new LocationAuth()
                        {
                            Userid = item.Id
                        });
                    }
                }

                Account.Locations.Add(newLocation);
            }
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/BusinessProfile/Locations/Index", new { message = "Location Updated" });
        }

        public class LocationViewModel
        {
            public LocationViewModel()
            {
                this.CostCenters = new List<CostCenterViewModel>();
                this.Users = new List<UserViewModel>();
            }

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

            public List<UserViewModel> Users { get; set; }

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
            public string Name { get; set; }
            public string UserId { get; set; }
            public string AuthorizerId { get; set; }
            public string AuthorizerEmail { get; set; }
            public string AuthorizerName { get; set; }
            public string AuthorizerSurname { get; set; }
            public string AuthorizerDisplayname
            {
                get
                {
                    return string.Format("{0} {1}", this.AuthorizerName, this.AuthorizerSurname);
                }
            }
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