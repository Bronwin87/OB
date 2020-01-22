using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Domain.Models;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Accounts
{
    public class GuestUserRegisterModel : BasePage
    {
        public GuestUserRegisterModel() { }

        [BindProperty]
        public GuestUserRegisterViewModel Input { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync(
            [FromServices] SetCartAddress setCartAddress,
            [FromServices] UserManager<ApplicationUser> userManager,
            [FromServices] SignInManager<ApplicationUser> signInManager)
        {
            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email
            };

            foreach (var item in userManager.PasswordValidators)
            {
                var password = await item.ValidateAsync(userManager, user, Input.Password);
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

            var address = new Address
            {
                Address1 = Input.Address1,
                Address2 = Input.Address2,
                City = Input.City,
                PostCode = Input.PostCode,
            };

            user.Addresses.Add(address);

            var result = await userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim("type", "user"));
                await signInManager.SignInAsync(user, false);

                await setCartAddress.Do(new SetCartAddress.Request
                {
                    UserMark = GetCartUserMark(),
                    AddressId = address.Id
                });

                return RedirectToPage("/Checkout/Payment");
            }
            return RedirectToPage("/Index");
        }
    }

    public class GuestUserRegisterViewModel
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


        [Required]
        public string LocationType { get; set; }
        public string Company { get; set; }
        [Required]
        public string RecieversFirstName { get; set; }
        [Required]
        public string RecieversLastName { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string PostCode { get; set; }
    }
}