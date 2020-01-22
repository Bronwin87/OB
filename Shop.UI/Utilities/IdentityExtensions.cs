using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace Shop.UI.Utilities
{
    public class CustomClaimsCookieSignInHelper<TIdentityUser> where TIdentityUser : IdentityUser
    {
        private readonly SignInManager<TIdentityUser> _signInManager;

        public CustomClaimsCookieSignInHelper(SignInManager<TIdentityUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public async Task SignInUserAsync(TIdentityUser user, bool isPersistent, IEnumerable<Claim> customClaims)
        {
            var claimsPrincipal = await _signInManager.CreateUserPrincipalAsync(user);
            if (customClaims != null && claimsPrincipal?.Identity is ClaimsIdentity claimsIdentity)
            {
                claimsIdentity.AddClaims(customClaims);
            }

            await _signInManager.Context.SignInAsync(IdentityConstants.ApplicationScheme,
                claimsPrincipal,
                new AuthenticationProperties { IsPersistent = isPersistent });
        }

    }
}
