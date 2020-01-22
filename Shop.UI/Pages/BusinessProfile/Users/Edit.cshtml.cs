using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile.Users
{
    public class EditModel : BasePage
    {
        private ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public EditModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
            Input = new EditUserViewModel();
        }

        public bool CanEdit { get; private set; }
        [BindProperty]
        public EditUserViewModel Input { get; set; }

        public IEnumerable<AssignableUser> AssignedUsers { get; set; }
        public IEnumerable<AssignableUser> AssignableUsers { get; set; }

        public IActionResult OnGet(string id)
        {
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            CanEdit = GetUserType() == "authorizer" || GetUserType() == "superuser";

            if (!CanEdit)
                return RedirectToPage("/BusinessProfile/Users/Index");

            var user = _context.Users
                .Include(x => x.AssignedUsers)
                .Include(x => x.Accounts)
                .FirstOrDefault(x => x.Id == id);

            if (user.Type == UserType.Authorizer)
            {
                var accountId = user.Accounts.FirstOrDefault(x => x.Active).AccountId;

                AssignableUsers = _context.Accounts
                    .Include(x => x.AccountUsers)
                        .ThenInclude(x => x.User)
                    .FirstOrDefault(x => x.Id == accountId)
                    .AccountUsers
                    .Select(x => x.User)
                    .Where(x => string.IsNullOrEmpty(x.AuthorizerId) && x.Type == UserType.BusinessUser)
                    .Select(x => new AssignableUser
                    {
                        Id = x.Id,
                        Email = x.Email
                    })
                    .ToList();

                AssignedUsers = user.AssignedUsers.Select(x => new AssignableUser
                {
                    Id = x.Id,
                    Email = x.Email
                });
            }

            Input.Id = user.Id;
            Input.FirstName = user.FirstName;
            Input.LastName = user.LastName;
            Input.Email = user.Email;
            Input.Type = user.Type;

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
            var user = _context.Users
                .Where(x => x.Id == Input.Id)
                .FirstOrDefault();

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Email = Input.Email;
            user.Type = Input.Type;

            var allClaims = await _userManager.GetClaimsAsync(user);
            Claim typeClaim = allClaims.FirstOrDefault(c => c.Type == "type");
            if (typeClaim != null)
            {
                await _userManager.RemoveClaimAsync(user, typeClaim);
            }

            await _userManager.AddClaimAsync(user, new Claim("type", Input.Type.GetClaim()));
            await _context.SaveChangesAsync();

            return RedirectToPage("/BusinessProfile/Users/Index");
        }

        public async Task<IActionResult> OnPostAssignUser(string userId)
        {
            var user = _context.Users
              .Where(x => x.Id == userId)
              .FirstOrDefault();

            user.AuthorizerId = Input.Id;

            await _context.SaveChangesAsync();
            return RedirectToPage("/BusinessProfile/Users/Index");
        }

        public async Task<IActionResult> OnPostUnAssignUser(string userId)
        {
            var user = _context.Users
              .Where(x => x.Id == userId)
              .FirstOrDefault();

            user.AuthorizerId = null;

            await _context.SaveChangesAsync();
            return RedirectToPage("/BusinessProfile/Users/Index");
        }
    }

    public class AssignableUser
    {
        public string Id { get; set; }
        public string Email { get; set; }
    }

    public class EditUserViewModel
    {
        public string Id { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public UserType Type { get; set; }
    }
}