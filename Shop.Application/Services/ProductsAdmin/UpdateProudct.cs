using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Shop.Database;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.ProductsAdmin
{
    public class UpdateProduct
    {
        private readonly string accessKey = "AKIAJ5KE3MDR3DRIYNEA";
        private readonly string secretKey = "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";

        private ApplicationDbContext _context;

        private readonly AmazonS3Client _amazonS3Client;
        private AmazonS3Config AmazonS3Config { get; set; }

        public UpdateProduct(ApplicationDbContext context)
        {
            _context = context;
            AmazonS3Config = new AmazonS3Config()
            {
                RegionEndpoint = RegionEndpoint.USEast2
            };
            _amazonS3Client = new AmazonS3Client(accessKey, secretKey, AmazonS3Config);
        }


        public async Task<bool> Do(Request request)
        {
            var product = _context.Products.FirstOrDefault(x => x.Id == request.Id);
            string newImgUrl = string.Empty;

            if (request.File != null)
            {
                try
                {
                    string imgKey = $"images/{request.Id}/{request.File.FileName}";
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

            product.Name = request.Name;
            product.Description = request.Description;
            product.Price = request.Value;
            product.Colour = request.Colour;
            product.CostPrice = request.CostPrice;
            product.ExternalId = request.ExternalId;
            product.SearchString = $"{request.Name} {request.ExternalId}";
            product.Unit = request.Unit;

            product.Published = request.Published;
            product.OutOfStock = request.OutOfStock;
            product.NoDiscount = request.NoDiscount;
            product.ValueAddedProduct = request.ValueAdded;

            product.MainCategoryId = request.Main;
            product.SubCategoryId = request.Sub;
            product.TertiaryCategoryId = request.Tri;

            if (!string.IsNullOrEmpty(newImgUrl))
            {
                product.ImageUrl = newImgUrl;
            }

            return await _context.SaveChangesAsync() > 0;
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
            public bool ValueAdded { get; set; }

            public string Main { get; set; }
            public string Sub { get; set; }
            public string Tri { get; set; }

            public IFormFile File { get; set; }
        }

        public class Response
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
            public decimal Value { get; set; }

            public bool NoDiscount { get; set; }
        }
    }
}
