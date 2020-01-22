using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Application.Services.Categories;
using System.Linq;
using Shop.UI.Utilities;
using Microsoft.Extensions.Caching.Memory;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System;

namespace Shop.UI.ViewComponents
{
    public class CategoryViewComponent : ViewComponent
    {
        private readonly string _menuCacheKey = "MainMenu";
        private readonly ApplicationDbContext _ctx;
        private IMemoryCache _cache;

        public CategoryViewComponent(ApplicationDbContext ctx, IMemoryCache memoryCache)
        {
            _ctx = ctx;
            _cache = memoryCache;
        }

        public IViewComponentResult Invoke()
        {
            List<MainCategory> mainMenu = new List<MainCategory>();
            // netcore caching
            // https://docs.microsoft.com/en-us/aspnet/core/performance/caching/memory?view=aspnetcore-2.2
            if (!_cache.TryGetValue(_menuCacheKey, out mainMenu))
            {
                // "MainMenu" key not in cache, get data from DB
                mainMenu = new GetCategories(_ctx).DoMenu().ToList();

                var cacheOptions = new MemoryCacheEntryOptions()
                                  // Sets cache entry size (only used if mimimum cache size is set in startup)
                                  //.SetSize(1)
                                  // Set's cache timeout period, resets if accessed
                                  .SetSlidingExpiration(TimeSpan.FromMinutes(30));

                // Save menu data in cache
                _cache.Set(_menuCacheKey, mainMenu, cacheOptions);
            }

            return View(mainMenu);
        }
    }
}
