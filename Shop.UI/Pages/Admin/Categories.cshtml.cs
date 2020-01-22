using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Application;
using Shop.Application.Services.Categories;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.Products;

namespace Shop.UI.Pages.Admin
{
    public class CategoriesModel : PageModel
    {
        private ApplicationDbContext _ctx;
        public CategoriesModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public List<MainCategory> MainCategories { get; set; }
        public void OnGet()
        {
            MainCategories = new GetCategories(_ctx).DoMenu().ToList();
        }
    }
}