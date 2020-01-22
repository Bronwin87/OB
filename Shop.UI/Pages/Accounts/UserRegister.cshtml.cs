using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Domain.Models;
using Shop.Domain.Models.Users;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;
using Shop.Application.Services.Emails;
using Shop.UI.Utilities;

namespace Shop.UI.Pages.Accounts
{
    public class UserRegisterModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private EmailSender _emailSender;
        private CookiesHelper _cookiesHelper;

        public UserRegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailSender emailSender,
            CookiesHelper cookiesHelper)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _cookiesHelper = cookiesHelper;
        }

        [BindProperty]
        public UserRegisterViewModel Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email,
                Type = Domain.Enums.UserType.IndividualUser
            };

            //foreach (var item in _userManager.PasswordValidators)
            //{
            //    var password = await item.ValidateAsync(_userManager, user, Input.Password);
            //    if (!password.Succeeded)
            //    {
            //        foreach (var error in password.Errors)
            //        {
            //            ModelState.AddModelError("Input.Password", string.Format("[{0}] - {1}", error.Code, error.Description));
            //        }
            //    }
            //}

            var result = await _userManager.CreateAsync(user, Input.Password);
            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("type", "user"));

                await _signInManager.SignInAsync(user, false);

                //send registration email
                // TODO - send customer email?
                //await _emailSender.SendTransactionalEmail(user.Email);

                _cookiesHelper.Set("PreviouslyLoggedInUser", "true", 365);
                return RedirectToPage("/Profile/Index", new { welcome = true });
            }
            return Page();
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
    }
}