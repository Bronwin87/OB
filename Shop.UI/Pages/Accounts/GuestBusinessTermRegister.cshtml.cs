using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Accounts;
using Shop.Application.Cart;
using Shop.Application.Oders;
using Shop.Application.Services.Emails;
using Shop.Database;
using Shop.Domain.Models.Users;
using Shop.UI.Infrastructure;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Accounts
{
    public class GuestBusinessTermRegisterModel : BasePage
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private ApplicationDbContext _ctx;
        private EmailSender _emailSender;

        public GuestBusinessTermRegisterModel(
            ApplicationDbContext ctx,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            EmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _ctx = ctx;
            _emailSender = emailSender;
        }

        [BindProperty]
        public CreateAccount.BusinessRegisterViewModel Input { get; set; }

        [BindProperty]
        public BusinessTermRegisterViewModel TermInput { get; set; }

        public void OnGet() { }

        public async Task<IActionResult> OnPostAsync()
        {
            if (String.IsNullOrEmpty(Input.References[2].CompanyName))
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

            var user = new ApplicationUser
            {
                FirstName = Input.FirstName,
                LastName = Input.LastName,
                UserName = Input.Email,
                Email = Input.Email,
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

            Input.TradingName = TermInput.TradingName;
            Input.AvgMonthly = TermInput.AvgMonthly;
            Input.AgreeTOC = TermInput.AgreeTOC;

            var existingUser = _ctx.Users
                .Include(x => x.Accounts)
                .FirstOrDefault(x => x.Email == Input.Email);

            if (existingUser != null)
            {
                var canSignIn = await _signInManager.CheckPasswordSignInAsync(existingUser, Input.Password, false);

                if (canSignIn.Succeeded)
                {
                    await _signInManager.SignInAsync(existingUser, false);

                    var accountId = await new CreateAccount(_ctx, _emailSender)
                                .Do(new CreateAccount.Request
                                {
                                    UserId = existingUser.Id,
                                    Input = Input
                                });

                    await SetCartAddress(accountId);

                    await PlaceOrder(accountId, existingUser.Id);

                    return RedirectToPage("/Index");
                }
                return Page();
            }

            var result = await _userManager.CreateAsync(user, Input.Password);

            if (result.Succeeded)
            {
                await _userManager.AddClaimAsync(user, new Claim("type", "superuser"));

                await _signInManager.SignInAsync(user, false);

                var accountId = await new CreateAccount(_ctx, _emailSender)
                            .Do(new CreateAccount.Request
                            {
                                UserId = user.Id,
                                Input = Input
                            });

                await SetCartAddress(accountId);

                await PlaceOrder(accountId, user.Id);
            }

            return RedirectToPage("/Accounts/GuestBusinessTermProfile");
        }

        public class BusinessTermRegisterViewModel
        {
            public string TradingName { get; set; }
            public string AvgMonthly { get; set; }
            public bool AgreeTOC { get; set; }
        }

        private async Task SetCartAddress(int accountId)
        {
            var address = _ctx.Accounts
                 .Include(x => x.Address)
                 .FirstOrDefault(x => x.Id == accountId)
                 .Address;

            await new SetCartAddress(_ctx).Do(new SetCartAddress.Request
            {
                UserMark = GetCartUserMark(),
                AddressId = address.Id
            });
        }

        private async Task PlaceOrder(int accountId, string userId)
        {
            var mark = GetCartUserMark();

            mark.userId = userId;

            await new CreateOrder(_ctx).Do(new CreateOrder.Request
            {
                UserMark = mark,
                AccountId = accountId,
                PreserveCart = true,
                Term = true,
            });
        }
    }
}