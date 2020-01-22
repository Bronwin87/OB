using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile.Users
{
    public class IndexModel : PageModel
    {
        private ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;

        public IndexModel(
            ApplicationDbContext ctx,
            UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        public bool CanEdit { get; private set; }
        public string Message { get; set; }

        [BindProperty]
        public UserRegisterViewModel Input { get; set; }

        public IEnumerable<ApplicationUser> Users { get; set; }

        public void OnGet(string message)
        {
            CanEdit = User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "authorizer"
            || User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "superuser";

            Message = message;

            Users = _ctx.AccountUsers
                .Include(x => x.Account)
                .ThenInclude(x => x.AccountUsers)
                .ThenInclude(x => x.User)
                .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                .Account
                .AccountUsers
                .Select(x => x.User);
        }

        public async Task<IActionResult> OnPost()
        {
            var usr = _ctx.Users.FirstOrDefault(u => u.Email == Input.Email);
            if (usr != null)
            {
                ModelState.AddModelError("Input.Email", "User already exists");
            }

            if (!ModelState.IsValid)
            {
                CanEdit = User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "authorizer"
                       || User.Claims.FirstOrDefault(x => x.Type == "type")?.Value == "superuser";

                Users = _ctx.AccountUsers
                    .Include(x => x.Account)
                    .ThenInclude(x => x.AccountUsers)
                    .ThenInclude(x => x.User)
                    .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                    .Account
                    .AccountUsers
                    .Select(x => x.User);

                return Page();
            }

            var accounId = _ctx.AccountUsers
                .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                .AccountId;

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email,
                Type = Input.Type,
            };

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                var accountUser = new AccountUser
                {
                    AccountId = accounId,
                    UserId = user.Id,
                    Active = true
                };

                _ctx.AccountUsers.Add(accountUser);

                await _ctx.SaveChangesAsync();

                await _userManager.AddClaimAsync(user, new Claim("type", Input.Type.GetClaim()));

                return RedirectToPage("/BusinessProfile/Users/Index", new { message = "User created" });
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            var favouritesLists = _ctx.FavouriteLists.Where(f => f.UserId == id)
                                      .ToList();

            foreach (var item in favouritesLists)
            {
                _ctx.FavouriteLists.Remove(item);
            }
            await _ctx.SaveChangesAsync();

            await _userManager.DeleteAsync(user);

            return RedirectToPage("/BusinessProfile/Users/Index", new { message = "User Deleted!" });
        }
    }

    public class UserRegisterViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        public UserType Type { get; set; }
    }
}