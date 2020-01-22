using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.ProductsAdmin.Entities;
using Shop.Database;
using System.Linq;

namespace Shop.Application.ProductsAdmin
{
    public class GetProduct
    {
        private ApplicationDbContext _ctx;

        public GetProduct(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public ProductViewModel Do(string id) =>
            _ctx.Products
                    .Include(x => x.Links)
                    .ThenInclude(x => x.Target)
                    .Where(x => x.Id == id)
                    .Select(x => new ProductViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Value = x.Price ?? 0,
                        Published = x.Published,
                        OutOfStock = x.OutOfStock,
                        ValueAdd = x.ValueAddedProduct,
                        SearchString = x.SearchString,
                        CostPrice = x.CostPrice,
                        ImageUrl = x.ImageUrl,
                        Main = x.MainCategoryId,
                        Sub = x.SubCategoryId,
                        Tri = x.TertiaryCategoryId,
                        ExternalId = x.ExternalId,
                        Unit = x.Unit,
                        Colour = x.Colour,
                        NoDiscount = x.NoDiscount ?? false,
                        Linked = x.Links.AsQueryable()
                            .Where(y => y.Type == Domain.Enums.ProductLinkType.Relation)
                            .Select(y => new ProductViewModel
                            {
                                Id = y.Target.Id,
                                ExternalId = y.Target.ExternalId,
                                Name = y.Target.Name,
                                Value = y.Target.Price ?? 0,
                                Colour = y.Target.Colour,
                                Brand = y.Target.Brand,
                            }).ToList(),
                        Alternative = x.Links.AsQueryable()
                            .Where(y => y.Type == Domain.Enums.ProductLinkType.Alternative)
                            .Select(y => new ProductViewModel
                            {
                                Id = y.Target.Id,
                                ExternalId = y.Target.ExternalId,
                                Name = y.Target.Name,
                                Value = y.Target.Price ?? 0,
                                Colour = y.Target.Colour,
                                Brand = y.Target.Brand,
                            }).ToList(),
                        ValueAdded = x.Links.AsQueryable()
                            .Where(y => y.Type == Domain.Enums.ProductLinkType.ValueAdded)
                            .Select(y => new ProductViewModel
                            {
                                Id = y.Target.Id,
                                ExternalId = y.Target.ExternalId,
                                Name = y.Target.Name,
                                Value = y.Target.Price ?? 0,
                                Colour = y.Target.Colour,
                                Brand = y.Target.Brand,
                            }).ToList()
                    })
                    .FirstOrDefault();
    }
}