using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Accounts;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models.Users;
using Shop.UI.Utilities;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Accounts
{
    public class BusinessRegisterModel : PageModel
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private ApplicationDbContext _ctx;
        private EmailSender _emailSender;
        private CookiesHelper _cookiesHelper;

        public BusinessRegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext ctx,
            EmailSender emailSender,
            CookiesHelper cookiesHelper)

        {
            _emailSender = emailSender;
            _userManager = userManager;
            _signInManager = signInManager;
            _ctx = ctx;
            _cookiesHelper = cookiesHelper;
            Input = new CreateAccount.BusinessRegisterViewModel();
        }

        [BindProperty()]
        public CreateAccount.BusinessRegisterViewModel Input { get; set; }

        //[Required]
        //[BindProperty]
        //public string ContractNumber { get; set; }

        public void OnGet()
        {
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!Input.TermAccount)
            {
                ModelState.ClearValidationState("Input.CreditLimit");
                ModelState.ClearValidationState("Input.RegistrationNumber");
                ModelState.ClearValidationState("Input.CompanyVAT");
                ModelState.MarkFieldValid("Input.CreditLimit");
                ModelState.MarkFieldValid("Input.RegistrationNumber");
                ModelState.MarkFieldValid("Input.CompanyVAT");

                for (int i = 0; i < 3; i++)
                {
                    ModelState.ClearValidationState($"Input.References[{i}].Email");
                    ModelState.ClearValidationState($"Input.References[{i}].SureName");
                    ModelState.ClearValidationState($"Input.References[{i}].FirstName");
                    ModelState.ClearValidationState($"Input.References[{i}].Telephone");
                    ModelState.ClearValidationState($"Input.References[{i}].CompanyName");
                    ModelState.MarkFieldValid($"Input.References[{i}].Email");
                    ModelState.MarkFieldValid($"Input.References[{i}].SureName");
                    ModelState.MarkFieldValid($"Input.References[{i}].FirstName");
                    ModelState.MarkFieldValid($"Input.References[{i}].Telephone");
                    ModelState.MarkFieldValid($"Input.References[{i}].CompanyName");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Input.References[2].CompanyName))
                {
                    ModelState.ClearValidationState("Input.References[2].Email");
                    ModelState.ClearValidationState("Input.References[2].SureName");
                    ModelState.ClearValidationState("Input.References[2].FirstName");
                    ModelState.ClearValidationState("Input.References[2].Telephone");
                    ModelState.ClearValidationState("Input.References[2].CompanyName");
                    ModelState.MarkFieldValid("Input.References[2].Email");
                    ModelState.MarkFieldValid("Input.References[2].SureName");
                    ModelState.MarkFieldValid("Input.References[2].FirstName");
                    ModelState.MarkFieldValid("Input.References[2].Telephone");
                    ModelState.MarkFieldValid("Input.References[2].CompanyName");
                }
            }

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email,
                PhoneNumber = Input.PhoneNumber,
                Type = Domain.Enums.UserType.SuperUser
            };

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


            //Input.ContractNumber = ContractNumber;
            //Input.ContractNumber = Input.PhoneNumber;

            var existingUser = _ctx.Users
                .Include(x => x.Accounts)
                .FirstOrDefault(x => x.Email == Input.Email);

            if (existingUser != null)
            {
                var canSignIn = await _signInManager.CheckPasswordSignInAsync(existingUser, Input.Password, false);

                if (canSignIn.Succeeded)
                {
                    await _signInManager.SignInAsync(existingUser, false);
                    await new CreateAccount(_ctx, _emailSender)
                            .Do(new CreateAccount.Request { UserId = existingUser.Id, Input = Input });

                    _cookiesHelper.Set("PreviouslyLoggedInUser", "true", 365);
                    if (Input.TermAccount)
                    {
                        return RedirectToPage("/Accounts/BusinessRegistered");
                    }
                    else
                    {
                        return RedirectToPage("/BusinessProfile/Index");
                    }
                }
                return Page();
            }

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("type", "superuser"));
                await _signInManager.SignInAsync(user, false);
                await new CreateAccount(_ctx, _emailSender)
                     .Do(new CreateAccount.Request
                     {
                         UserId = user.Id,
                         Input = Input
                     });

                _cookiesHelper.Set("PreviouslyLoggedInUser", "true", 365);
                if (Input.TermAccount)
                {
                    return RedirectToPage("/Accounts/BusinessRegistered");
                }
                else
                {
                    return RedirectToPage("/BusinessProfile/Index");
                }
            }

            return Page();
        }
    }
}