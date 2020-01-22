using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Shop.Domain.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Accounts
{
    public class ResetPasswordModel : PageModel
    {

        private IConfiguration _config;
        private UserManager<ApplicationUser> _userManager;

        public ResetPasswordModel(IConfiguration config, UserManager<ApplicationUser> userManger)
        {
            _config = config;
            _userManager = userManger;
        }

        [BindProperty]
        public ResetPasswordViewModel Input { get; set; }

        public async Task OnGet(string code = null, string email = null)
        {
            Input = new ResetPasswordViewModel()
            {
                Code = code,
                Email = email,
                EmailPrepopulated = !string.IsNullOrEmpty(email)
            };

            //if (Input.EmailPrepopulated)
            //{
            //    var user = await _userManager.FindByEmailAsync(Input.Email);
            //    if(user == null)
            //    {
            //        ModelState.AddModelError("Input.Email", "Your email was prepopulated, but does not exist in the database.");
            //    }
            //    else
            //    {
            //        Input.Code = await _userManager.GeneratePasswordResetTokenAsync(user);
            //    }
            //}
        }

        public async Task<IActionResult> OnPost()
        {
            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                return RedirectToPage("ResetConfirmation");
            }

            foreach (var item in _userManager.PasswordValidators)
            {
                var password = await item.ValidateAsync(_userManager, user, Input.Password);
                if (!password.Succeeded)
                {
                    foreach (var error in password.Errors)
                    {
                        ModelState.AddModelError("Input.Password", string.Format("[{0}] - {1}", error.Code, error.Description));
                    }
                }
            }

            if (!ModelState.IsValid)
                return Page();

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("ResetConfirmation");
            }
            return Page();
        }
    }
    public class ResetPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }

        public bool EmailPrepopulated { get; set; }
    }
}