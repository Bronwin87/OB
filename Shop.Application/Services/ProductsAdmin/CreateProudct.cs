using Microsoft.AspNetCore.Http;
using Shop.Application.Services.ProductsAdmin.Entities;
using Shop.Database;
using Shop.Domain.Models.Products;
using System.Threading.Tasks;
using Amazon.S3;
using System.IO;
using Amazon.S3.Model;
using System;
using Amazon;

namespace Shop.Application.ProductsAdmin
{
    public class CreateProduct
    {
        private readonly string accessKey = "AKIAJ5KE3MDR3DRIYNEA";
        private readonly string secretKey = "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";

        private readonly AmazonS3Client _amazonS3Client;
        private AmazonS3Config AmazonS3Config { get; set; }

        private ApplicationDbContext _context;

        public CreateProduct(ApplicationDbContext context)
        {
            _context = context;
            AmazonS3Config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.USEast2
            };
            _amazonS3Client = new AmazonS3Client(accessKey, secretKey, AmazonS3Config);
        }

        public async Task<ProductViewModel> Do(Request request)
        {
            var product = new Product
            {
                Name = request.Name,
                Description = request.Description,
                SearchString = $"{request.Name} {request.ExternalId}",
                Price = request.Value,
                CostPrice = request.CostPrice,
                ImageUrl = request.ImageUrl,
                ExternalId = request.ExternalId,
                Unit = request.Unit,
                Colour = request.Colour,
                NoDiscount = request.NoDiscount,
                Published = request.Published,
                OutOfStock = request.OutOfStock,

                MainCategoryId = request.Main,
                SubCategoryId = request.Sub,
                TertiaryCategoryId = request.Tri
            };

            _context.Products.Add(product);

            await _context.SaveChangesAsync();

            string newImgUrl = string.Empty;
            if (request.File != null)
            {
                try
                {
                    string imgKey = $"images/{product.Id}/{request.File.FileName}";
                    var ms = new MemoryStream();
                    request.File.CopyTo(ms);
                    ms.Seek(0, SeekOrigin.Begin);

                    PutObjectRequest putObjectRequest = new PutObjectRequest()
                    {
                        BucketName = "officebox.resources",
                        Key = imgKey,
                        InputStream = ms,
                        ContentType = request.File.ContentType,
                        CannedACL = S3CannedACL.PublicRead
                    };  

                    await _amazonS3Client.PutObjectAsync(putObjectRequest);

                    newImgUrl = $"https://s3.us-east-2.amazonaws.com/officebox.resources/{imgKey}";
                }
                catch (Exception e)
                {
                    throw e;
                }
            }

            if (!string.IsNullOrEmpty(newImgUrl))
            {
                product.ImageUrl = newImgUrl;
            }

            await _context.SaveChangesAsync();

            return new GetProduct(_context).Do(product.Id);
        }

        public class Request
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public decimal CostPrice { get; set; }
            public string ImageUrl { get; set; }
            public string ExternalId { get; set; }
            public string Unit { get; set; }
            public string Colour { get; set; }
            public string Description { get; set; }
            public decimal Value { get; set; }

            public bool Published { get; set; }
            public bool OutOfStock { get; set; }
            public bool NoDiscount { get; set; }

            public string Main { get; set; }
            public string Sub { get; set; }
            public string Tri { get; set; }

            public IFormFile File { get; set; }
        }
    }
}
