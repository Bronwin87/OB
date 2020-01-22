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

namespace Shop.UI.Pages.BusinessProfile.MultipleLocations
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

            if (CanEdit)
            {
                //to view multiple locations for this account
                Account = _ctx.AccountUsers
                    .Include(x => x.Account)
                        .ThenInclude(x => x.Locations)
                        .ThenInclude(x => x.Address)
                    .Include(x => x.Account)
                        .ThenInclude(x => x.Locations)
                        .ThenInclude(x => x.CostCenters)
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
                            AuthorizerId = y.AuthorizerId
                        })
                    });

                Users = Account.AccountUsers.Where(x => x.User.Type != Domain.Enums.UserType.Authorizer).Select(x => x.User);
                Authorizers = Account.AccountUsers.Where(x => x.User.Type == Domain.Enums.UserType.Authorizer).Select(x => x.User);
            }
            else
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;

                UserLocations = _ctx.Locations
                    .Include(x => x.Address)
                    .Include(x => x.User)
                    .Include(x => x.Authorizer)
                    .Where(x => x.UserId == userId)
                    .ToList();
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
                var Location = _ctx.Locations.Include(x => x.Address).FirstOrDefault(x => x.Id == Input.Id);

                Location.Name = Input.CompanyName;
                Location.Address.Address1 = Input.Address1;
                Location.Address.Address2 = Input.Address2;
                Location.Address.City = Input.City;
                Location.Address.PostCode = Input.PostCode;
                Location.PhoneNumber = Input.PhoneNumber;
                Location.UserId = Input.UserId;
                Location.AuthorizerId = Input.AuthorizerId;
            }
            else
            {
                Account = _ctx.Accounts.FirstOrDefault(x => x.Id == Input.AccountId);

                Account.Locations.Add(new Location
                {
                    Name = Input.CompanyName,
                    PhoneNumber = Input.PhoneNumber,
                    UserId = Input.UserId,
                    AuthorizerId = Input.AuthorizerId,
                    Address = new Domain.Models.Address
                    {
                        Address1 = Input.Address1,
                        Address2 = Input.Address2,
                        City = Input.City,
                        PostCode = Input.PostCode,
                        Country = "SOUTH AFRICA"
                    }
                });
            }

            await _ctx.SaveChangesAsync();

            return RedirectToPage("/BusinessProfile/MultipleLocations/Index", new { message = "Location Updated" });
        }

        public async Task<IActionResult> OnPostCostCenter()
        {
            var Location = _ctx.Locations.FirstOrDefault(x => x.Id == Input2.LocationId);
            Location.CostCenters.Add(new CostCenter
            {
                Name = Input2.Name
            });
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/BusinessProfile/MultipleLocations/Index", new { message = "Cost Center Updated" });
        }

        public async Task<IActionResult> OnPostDeleteCostCenter(int id)
        {
            var costCenter = _ctx.CostCenters.FirstOrDefault(x => x.Id == id);
            _ctx.Remove(costCenter);
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/BusinessProfile/MultipleLocations/Index", new { message = "Cost Center Deleted" });
        }

        public async Task<IActionResult> OnPostEditCostCenter()
        {
            var OldCostCenters = _ctx.CostCenters
                .Include(x => x.Location)
                .Include(x => x.User)
                .Where(x => x.LocationId != Input2.LocationId && x.UserId == Input2.UserId && x.User.Type == Domain.Enums.UserType.BusinessUser)
                .ToList();

            if (OldCostCenters.Count > 0)
                foreach (var cc in OldCostCenters)
                    cc.UserId = null;

            var OldLocation = _ctx.Locations
                .Include(x => x.User)
                .FirstOrDefault(x => x.UserId == Input2.UserId && x.User.Type == Domain.Enums.UserType.BusinessUser);

            if (OldLocation != null)
                OldLocation.UserId = null;

            var costCenter = _ctx.CostCenters.FirstOrDefault(x => x.Id == Input2.Id);
            costCenter.Name = Input2.Name;
            costCenter.UserId = Input2.UserId;
            costCenter.AuthorizerId = Input2.AuthorizerId;
            await _ctx.SaveChangesAsync();
            return RedirectToPage("/BusinessProfile/MultipleLocations/Index", new { message = "Cost Center Updated" });
        }
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
        public string AuthorizerId { get; set; }
    }
}