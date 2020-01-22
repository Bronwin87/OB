using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.Timezone;
using Shop.Database;
using Shop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Shop.Application.Oders
{
    public class GetOrder
    {
        private ApplicationDbContext _ctx;

        public GetOrder(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Response
        {
            public int Id { get; set; }
            public string OrderRef { get; set; }
            public OrderStatus Status { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string PhoneNumber { get; set; }
            public string OrderNumber { get; set; }
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string PostCode { get; set; }
            public DateTime Created { get; set; }
            public int Cin7ID { get; set; }
            public decimal Discount { get; set; }
            public decimal OriginalSubtotal { get; set; }
            public int ProductCount { get; set; }
            public decimal Subtotal { get; set; }
            public decimal VAT { get; set; }
            public int AccountId { get; set; }
            public IEnumerable<Product> Products { get; set; }
            public Domain.Models.Accounts.Account Account { get; set; }
            public Domain.Models.Accounts.Location Location { get; set; }
            public Domain.Models.Accounts.CostCenter CostCenter { get; set; }
            public string TotalValue { get; set; }
            public double Delivery { get; set; }
            public int ? LocationId { get; set; }
            public int ? CostcenterId { get; set; }
        }

        public class Product
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public string Value { get; set; }
            public int Qty { get; set; }
            public string StockDescription { get; set; }
            public string ExternalId { get; set; }
            public string UOM { get; set; }
            public string Colour { get; set; }
        }
        public Response Do(string orderNumber) =>
           _ctx.Orders
               .Where(x => x.OrderNumber == orderNumber)
               .Include(x => x.OrderProducts)
               .ThenInclude(x => x.Product)
               .Include(x => x.Address)
               .Include(x => x.User)
                .Include(x => x.Account)
               .Select(x => new Response
               {
                   Id = x.Id,
                   OrderRef = x.OrderRef,
                   OrderNumber = x.OrderNumber,
                   FirstName = x.User.FirstName,
                   LastName = x.User.LastName,
                   Email = x.User.Email,
                   PhoneNumber = x.User.PhoneNumber,
                   Address1 = x.Address.Address1,
                   Address2 = x.Address.Address2,
                   City = x.Address.City,
                   PostCode = x.Address.PostCode,
                   Status = x.Status,
                   TotalValue = x.SubTotal.ToString("N2"),

                   Created = x.Created.ConvertTimeToLocal(),
                    Delivery = x.Delivery,
                     Discount = x.Discount,
                     //AccountId = x.AccountId,
                      OriginalSubtotal = x.OriginalSubtotal,
                       ProductCount = x.ProductCount,
                        Subtotal = x.SubTotal,
                         VAT = x.Vat,
                         CostcenterId = x.CostCenterId,
                         LocationId = x.LocationId,
                   Products = x.OrderProducts.Select(y => new Product
                   {

                        //todo populate order
                    }),
                })
               .FirstOrDefault();
    }
}