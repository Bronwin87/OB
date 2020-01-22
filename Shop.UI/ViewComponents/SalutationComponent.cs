using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Shop.Database;
using Shop.UI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.ViewComponents
{
    public class SalutationViewComponent : ViewComponent
    {
        private readonly ApplicationDbContext _ctx;
        private IMemoryCache _cache;

        public SalutationViewComponent(ApplicationDbContext ctx, IMemoryCache memoryCache)
        {
            _ctx = ctx;
            _cache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            SalutationViewModel salutationModel = new SalutationViewModel();
            // netcore caching
            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2
            if (User.Identity.IsAuthenticated)
            {
                if (!_cache.TryGetValue(User.Identity.Name, out salutationModel))
                {
                    var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var usr = _ctx.Users.Find(userId);

                    salutationModel = new SalutationViewModel()
                    {
                        UserId = userId,
                        Email = usr.Email,
                        FirstName = usr.FirstName,
                        LastName = usr.LastName
                    };

                    var cacheOptions = new MemoryCacheEntryOptions()
                                  // Sets cache entry size (only used if mimimum cache size is set in startup)
                                  //.SetSize(1)
                                  // Set's cache timeout period, resets if accessed
                                  .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                    // Save menu data in cache
                    _cache.Set(User.Identity.Name, salutationModel, cacheOptions);
                }
            }

            return View(salutationModel);
        }
    }
}
