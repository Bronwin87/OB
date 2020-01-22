using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System.Linq;


namespace Shop.Application.Services.Categories
{
    public class GetCategories
    {
        private ApplicationDbContext _context;

        public GetCategories(ApplicationDbContext context)
        {
            _context = context;
            _context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public IEnumerable<MainCategory> Do()
        {
            return _context.MainCategories
                .Include(x => x.SubCategories)
                .ThenInclude(x => x.TertiaryCategories)
                .ToList();
        }

        public IEnumerable<Category> DoAdmin()
        {
            //this can be done faster by linking in code
            return _context.MainCategories
                    .Include(x => x.SubCategories)
                    .ThenInclude(x => x.TertiaryCategories)
                    .Select(x => new Category
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Categories = x.SubCategories
                            .Select(y => new Category
                            {
                                Id = y.Id,
                                Name = y.Name,
                                Categories = y.TertiaryCategories
                                    .Select(z => new Category
                                    {
                                        Id = z.Id,
                                        Name = z.Name,
                                    })
                            })
                    })
                    .ToList();
        }

        public IEnumerable<MainCategory> DoMenu()
        {
            var cats = _context.MainCategories
                               .Include(x => x.SubCategories)
                               .ThenInclude(x => x.TertiaryCategories)
                               .Where(c => !c.Deleted)
                               .Select(c => new MainCategory()
                               {
                                   Id = c.Id,
                                   Name = c.Name,
                                   CamelName = c.CamelName,
                                   Order = c.Order,
                                   Deleted = c.Deleted,
                                   SubCategories = c.SubCategories.Where(sc => !sc.Deleted).Select(sc => new SubCategory()
                                   {
                                       Id = sc.Id,
                                       Name = sc.Name,
                                       ImageUrl = sc.ImageUrl,
                                       MainCategoryId = sc.MainCategoryId,
                                       Deleted = sc.Deleted,
                                       TertiaryCategories = sc.TertiaryCategories.Where(tc => !tc.Deleted).Select(tc => new TertiaryCategory()
                                       {
                                           Id = tc.Id,
                                           Name = tc.Name,
                                           ImageUrl = tc.ImageUrl,
                                           SubCategoryId = tc.SubCategoryId,
                                           Deleted = tc.Deleted
                                       }).ToList()
                                   }).ToList()
                               })
                               .OrderBy(c => c.Order);
            return cats;
        }

        public class Category
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public IEnumerable<Category> Categories { get; set; }
        }
    }


}
