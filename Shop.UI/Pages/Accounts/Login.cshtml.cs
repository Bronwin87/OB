using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Shop.UI.Utilities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Shop.Database;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Cart;

namespace Shop.UI.Pages.Accounts
{
    public class LoginModel : BasePage
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private CustomClaimsCookieSignInHelper<ApplicationUser> _customClaimsCookieSignInHelper;
        private CookiesHelper _cookiesHelper;
        private ApplicationDbContext _ctx;
        private AddToCart _addToCart;

        public LoginModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            CustomClaimsCookieSignInHelper<ApplicationUser> customClaimsCookieSignInHelper,
            CookiesHelper cookiesHelper,
            ApplicationDbContext ctx,
            AddToCart addToCart)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _customClaimsCookieSignInHelper = customClaimsCookieSignInHelper;
            _cookiesHelper = cookiesHelper;
            _ctx = ctx;
            _addToCart = addToCart;
        }

        [BindProperty]
        public LoginViewModel Input { get; set; }

        public void OnGet(string ReturnUrl)
        {
            if (!string.IsNullOrEmpty(ReturnUrl))
            {
                Input = new LoginViewModel()
                {
                    ReturnUrl = ReturnUrl
                };
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var user = await _userManager.FindByEmailAsync(Input.Username);

            if (user == null)
            {
                ModelState.AddModelError("Input.Username", "Incorrect username or password.");
                return Page();
            }

            var result = await _signInManager.PasswordSignInAsync(user, Input.Password, false, false);

            IdentityUser usr = new IdentityUser();

            if (result.Succeeded)
            {
                // Add user id to current session cart
                // Remove session id from cart at the same time
                var sessionId = GetSessionId();
                var cart = _ctx.Carts.Include(c => c.Products).FirstOrDefault(x => x.SessionId == sessionId);

                if (cart != null)
                {
                    var existingCart = _ctx.Carts.Include(c => c.Products).FirstOrDefault(x => x.UserId == user.Id);
                    // if user already has a cart, add products to the existing cart
                    if (existingCart != null)
                    {
                        foreach (var product in cart.Products.ToList())
                        {
                            // the product should not already exist in the cart
                            if (!existingCart.Products.Any(cp => cp.ProductId == product.ProductId))
                            {
                                await _addToCart.Do(new AddToCart.Request()
                                {
                                    ProductId = product.ProductId,
                                    Qty = product.Qty,
                                    UserMark = (user.Id, string.Empty)
                                });
                            }
                        }
                        _ctx.Carts.Remove(cart);
                    }
                    else
                    {
                        cart.SessionId = null;
                        cart.UserId = user.Id;
                    }
                    _ctx.SaveChanges();
                }

                HttpContext.Session.SetString("User_FirstName", user.FirstName);
                HttpContext.Session.SetString("User_LastName", user.LastName);
                HttpContext.Session.SetString("User_Id", user.Id);

                var customClaims = new[]
                {
                    new Claim("User_FirstName", user.FirstName),
                    new Claim("User_LastName", user.LastName)
                };

                _cookiesHelper.Set("PreviouslyLoggedInUser", "true", 365);
                await _customClaimsCookieSignInHelper.SignInUserAsync(user, false, customClaims);

                if (Input.RedirectToPayment)
                    return RedirectToPage("/Checkout/CustomerDetails");

                if (!string.IsNullOrEmpty(Input.ReturnUrl))
                    return Redirect(Input.ReturnUrl);

                return RedirectToPage("/Shop/Index");
            }
            else
            {
                return Page();
            }
        }
    }

    public class LoginViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public bool RedirectToPayment { get; set; }
    }
}