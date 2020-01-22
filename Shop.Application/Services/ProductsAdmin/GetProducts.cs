using Microsoft.EntityFrameworkCore;
using Shop.Application.Infrastructure;
using Shop.Database;
using Shop.Domain.Models.Products;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.ProductsAdmin
{
    public class GetProducts
    {
        private ApplicationDbContext _ctx;

        public GetProducts(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public class Request : PagedRequest
        {
            public string Search { get; set; }
            public bool NeedPublished { get; set; }
            public bool ValueAdded { get; set; }
        }

        public class Response : PagedRequest
        {
            public List<ProductViewModel> Products { get; set; }
        }

        public Response Do(Request request)
        {
            var query = ProductsQuery(request.Search);

            if (request.PerPage > 0)
                query = query
                    .Skip(request.PerPage * (request.Page - 1))
                    .Take(request.PerPage);

            if (request.NeedPublished)
                query = query.Where(x => x.Published);

            if (request.ValueAdded)
                query = query.Where(x => x.ValueAddedProduct);

            return new Response
            {
                Products = query
                    .Select(x => new ProductViewModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Value = x.Price ?? 0,
                        Colour = x.Colour,
                        CostPrice = x.CostPrice,
                        ImageUrl = x.ImageUrl,
                        NoDiscount = x.NoDiscount ?? false,
                        OutOfStock = x.OutOfStock,
                        Description = x.Description,
                        ExternalId = x.ExternalId,
                        Published = x.Published,
                        SearchString = x.SearchString,
                        Unit = x.Unit,
                        Brand = x.Brand
                    })
                    .ToList(),
                Total = ProductsQuery(request.Search).Count()
            };
        }

        private IQueryable<Product> ProductsQuery(string search) =>
            _ctx.Products.Where(x => EF.Functions.Like(x.SearchString, $"%{search}%"));


        public class ProductViewModel
        {
            public string Id { get; set; }
            public bool Published { get; set; }
            public bool OutOfStock { get; set; }
            public string Name { get; set; }
            public string SearchString { get; set; }
            public decimal CostPrice { get; set; }
            public string ImageUrl { get; set; }
            public int Category { get; set; }
            public string ExternalId { get; set; }
            public string Unit { get; set; }
            public string Colour { get; set; }
            public string Description { get; set; }
            public string Brand { get; set; }
            public decimal Value { get; set; }

            public bool NoDiscount { get; set; }
        }
    }
}
