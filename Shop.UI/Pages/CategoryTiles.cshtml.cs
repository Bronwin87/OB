using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Shop.Application.Cart;
using Shop.Application.Services.Categories;
using Shop.Application.Products;
using Shop.Application.Services.FavouriteLists;
using Shop.Database;
using Shop.Domain.Models;
using Shop.Domain.Models.Products;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Shop.Application.Products.GetProductSearch;
//using Shop.Application.Services.Products.Entities;
//using Shop.Application.Products;

namespace Shop.UI.Pages.Shop
{
    public class CategoryTilesModel : BasePage
    {
        private ApplicationDbContext _context;
        private IOptions<Discounts> _discounts;
        public CategoryTilesModel(ApplicationDbContext contetxt, IOptions<Discounts> discounts)
        {
            _context = contetxt;
            _discounts = discounts;
        }

        [BindProperty]
        public AddToCart.Request Input { get; set; }

        [BindProperty]
        public SortingParameter SortOrder { get; set; }


        public CategoryViewModel CategoryModel { get; set; }

        public IEnumerable<MainCategory> MainCats { get; set; }
        public IEnumerable<MainCategory> MainParentCatName { get; set; }
        public IEnumerable<SubCategory> SubCats { get; set; }
        public IEnumerable<TertiaryCategory> TerCats { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<GetLists.Response> UserLists { get; set; }
        public string TopCatName { get; set; }
        public string SortOrderValue { get; set; } = "AZ";
        public async Task<IActionResult> OnGet(string category, string id, string topName, string sortValue)
        {
            SortOrderValue = sortValue;
            if (SortOrderValue == "undefined")
            {
                SortOrderValue = "AZ";
            }
            //MainCats = new GetCategories(_context).Do();
            //TopCatName = topName;

            if (category == "main")
            {
                var mainCategory = _context.MainCategories.Where(m => m.Id == id)
                                                          .Include(m => m.SubCategories)
                                                          .FirstOrDefault();

                CategoryModel = new CategoryViewModel()
                {
                    Id = id,
                    Name = mainCategory.CamelName,
                    ChildCategories = mainCategory.SubCategories
                                                  .Where(s => !s.Deleted)
                                                  .OrderBy(s => s.Name)
                                                  .Select(s => new CategoryViewModel()
                                                  {
                                                      Id = s.Id,
                                                      Name = s.Name,
                                                      Type = "sub",
                                                      ImageUrl = s.ImageUrl
                                                      
                                                  })
                };
            }
            else if (category == "sub")
            {
                var subCategory = await _context.SubCategories.Where(s => s.Id == id)
                                                              .Include(s => s.MainCategory)
                                                              .Include(s => s.TertiaryCategories)
                                                              .FirstOrDefaultAsync();

                CategoryModel = new CategoryViewModel()
                {
                    Id = id,
                    Name = subCategory.Name,
                    ChildCategories = subCategory.TertiaryCategories
                                                 .Where(t => !t.Deleted)
                                                 .OrderBy(t => t.Name)
                                                 .Select(t => new CategoryViewModel()
                                                 {
                                                     Id = t.Id,
                                                     Name = t.Name,
                                                     Type = "tri",
                                                     ImageUrl = t.ImageUrl
                                                 }),
                    Path = new List<CategoryViewModel>()
                    {
                        new CategoryViewModel()
                        {
                            Id = subCategory.MainCategoryId,
                            Name = subCategory.MainCategory.CamelName,
                            Type = "main"
                        }
                    }
                };
            }
            else if (category == "tri")
            {
                var tertiaryCategory = await _context.TertiaryCategories.Where(t => t.Id == id)
                                                                        .Include(t => t.SubCategory)
                                                                        .ThenInclude(s => s.MainCategory)
                                                                        .FirstOrDefaultAsync();

                CategoryModel = new CategoryViewModel()
                {
                    Id = id,
                    Name = tertiaryCategory.Name,
                    Path = new List<CategoryViewModel>()
                    {
                        new CategoryViewModel()
                        {
                            Id = tertiaryCategory.SubCategory.MainCategory.Id,
                            Name = tertiaryCategory.SubCategory.MainCategory.CamelName,
                            Type = "main",
                            
                        },
                        new CategoryViewModel()
                        {
                            Id = tertiaryCategory.SubCategoryId,
                            Name = tertiaryCategory.SubCategory.Name,
                            Type = "sub"
                        }
                    }
                };
            }

            if (!CategoryModel.ChildCategories.Any())
            {
               
                // Query Products Only if no child category
                Products = new GetProductSearch(_context).Do(new GetProductSearchQuery { CategoryId = id, CategoryType = category });
            }

            UserLists = new GetLists(_context).Do(GetUserId());
            return Page();
        }

        public class CategoryViewModel
        {
            public CategoryViewModel()
            {
                this.Path = new List<CategoryViewModel>();
                this.ChildCategories = new List<CategoryViewModel>();
            }

            public string Id { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public IEnumerable<CategoryViewModel> Path { get; set; }
            public IEnumerable<CategoryViewModel> ChildCategories { get; set; }
            public string ImageUrl { get; set; }
            public string Keywords
            {
                get
                {
                    return string.Format(
                        "Stationery, online shopping, ecommerce, {0}",
                        string.Join(", ", this.Path.Select(p => p.Name).ToList())
                    );
                }
            }
        }

        public class SortingParameter
        {
            public string Order { get; set; }
           
        }
    }
}