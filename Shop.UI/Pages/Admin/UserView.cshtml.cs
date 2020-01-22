using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Database;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Shop.UI.Pages.Admin
{
    public class UserViewModel : BasePage
    {
        public ApplicationDbContext _ctx;
        private UserManager<ApplicationUser> _userManager;

        public UserViewModel(ApplicationDbContext ctx, UserManager<ApplicationUser> userManager)
        {
            _ctx = ctx;
            _userManager = userManager;
        }

        [BindProperty]
        public ApplicationUser User { get; set; }

        [BindProperty]
        public bool UpdatePassword { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        public string ConfirmPassword { get; set; }

        public void OnGet(string id)
        {
            User = _ctx.Users
                    .Include(u => u.Accounts)
                    .FirstOrDefault(u => u.Id == id);
        }

        public async Task<IActionResult> OnPost()
        {
            var usr = await _ctx.Users.FindAsync(User.Id);

            if (await _ctx.Users.AnyAsync(u => u.Id != User.Id && u.UserName.ToUpper() == User.UserName.ToUpper()))
            {
                ModelState.AddModelError("Username", "This username already exists.");
            }
            if (!string.IsNullOrEmpty(Password))
            {
                if (Password != ConfirmPassword)
                {
                    ModelState.AddModelError("Password", "The password and confirm password values do not match.");
                }
                else
                {
                    foreach (var item in _userManager.PasswordValidators)
                    {
                        var password = await item.ValidateAsync(_userManager, usr, Password);
                        if (!password.Succeeded)
                        {
                            foreach(var error in password.Errors)
                            {
                                ModelState.AddModelError("Password", string.Format("[{0}] - {1}", error.Code, error.Description));
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid)
            {
                usr.FirstName = User.FirstName;
                usr.LastName = User.LastName;
                usr.Email = User.Email;
                usr.UserName = User.UserName;
                usr.JobTitle = User.JobTitle;
                usr.Type = User.Type;

                var allClaims = await _userManager.GetClaimsAsync(usr);
                Claim typeClaim = allClaims.FirstOrDefault(c => c.Type == "type");
                if (typeClaim != null)
                {
                    await _userManager.RemoveClaimAsync(usr, typeClaim);
                }

                if (!string.IsNullOrEmpty(Password))
                { // Workaround to change the password without knowing the current password
                    string passwordResetCode = await _userManager.GeneratePasswordResetTokenAsync(usr);
                    var result = await _userManager.ResetPasswordAsync(usr, passwordResetCode, Password);
                }

                await _userManager.AddClaimAsync(usr, new Claim("type", User.Type.GetClaim()));
                await _ctx.SaveChangesAsync();

                return RedirectToPage("/Admin/UserView", new { id = User.Id });
            }

            return Page();
        }
    }
}