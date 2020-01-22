using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Domain.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile
{
    public class ChangePasswordModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;

        public ChangePasswordModel(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string Message { get; set; }

        [BindProperty]
        public PasswordChangeViewModel Input { get; set; }

        public void OnGet(string message)
        {
            Message = message;
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await GetCurrentUserAsync();

            foreach (var item in _userManager.PasswordValidators)
            {
                var password = await item.ValidateAsync(_userManager, user, Input.NewPassword);
                if (!password.Succeeded)
                {
                    foreach (var error in password.Errors)
                    {
                        ModelState.AddModelError("Input.NewPassword", string.Format("[{0}] - {1}", error.Code, error.Description));
                    }
                }
            }

            if (!ModelState.IsValid)
                return Page();

            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, Input.CurrentPassword, Input.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return RedirectToPage("/BusinessProfile/ChangePassword", new { message = "Password Updated" });
                }

                return Page();
            }

            return Page();
        }
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }
    }

    public class PasswordChangeViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword")]
        public string ConfirmPassword { get; set; }
    }
}