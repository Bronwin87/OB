using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Models.Users;
using System.Threading.Tasks;

namespace Shop.UI.Controllers.Admin
{
    public class AccountController : Controller
    {
        private SignInManager<ApplicationUser> _signInManager;

        public AccountController(SignInManager<ApplicationUser> signInManager)
        {
            _signInManager = signInManager;
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToPage("/Index");
        }

        public async Task<JsonResult> CheckEmailAddress(string emailAddress)
        {
            return Json(true);
        }
    }
}
