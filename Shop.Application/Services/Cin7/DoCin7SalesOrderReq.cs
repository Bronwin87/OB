using System;
using System.Collections.Generic;
using System.Text;
using Shop.Database;
using Shop.Application.Oders;
using Shop.Domain.Models;
using Cin7ApiWrapper.Models;
using Cin7ApiWrapper;
using Cin7ApiWrapper.Common;
using Cin7ApiWrapper.Infrastructure;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shop.Application.Services.Accounts;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Orders;
using Shop.Domain.Enums;
using Shop.Domain.Models.Users;

namespace Shop.Application.Services.Cin7
{
    public class DoCin7SalesOrderReq
    {
        public ApplicationDbContext _ctx;
        public DoCin7SalesOrderReq(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public string OrderNumber { get; set; }
        }

        public async Task<CreateResult> DoCin7Req(string id)
        {
            //make connection to CIN7
            var api = new Cin7Api(new ApiUser("StationeryinMotionSA", "1251ada5191046238405f5a66ebbac8f"));

            //Get Order
            var order = _ctx.Orders
                 .Include(x => x.OrderProducts)

                 .Include(x => x.Account)
                    .ThenInclude(x => x.Locations)
                 .Include(x => x.Address)
                 .Include(x => x.User)
                 .FirstOrDefault(x => x.OrderNumber == id);

            if (order.Account == null)
            {
                order = _ctx.Orders
                 .Include(x => x.OrderProducts)
                 .Include(x => x.Address)
                 .Include(x => x.User)
                 .FirstOrDefault(x => x.OrderNumber == id);
            }

            var products = _ctx.Products;
            Cin7ApiWrapper.Models.SalesOrder saleO = new Cin7ApiWrapper.Models.SalesOrder();

            saleO.CreatedDate = order.Created;
            saleO.ModifiedDate = order.Created;
            saleO.Reference = order.OrderNumber;

            saleO.CurrencyCode = "ZAR";
            saleO.CurrencySymbol = "R";
            saleO.TaxStatus = TaxStatus.Excl;
            saleO.TaxRate = 0.14m;
            saleO.Total = Convert.ToDecimal(order.SubTotal);

            if (order.Account != null)
            {
                saleO.BillingCompany = order.Account.CompanyName ;
                saleO.DeliveryCompany = order.Account.CompanyName ;
                saleO.Company = order.Account.CompanyName;
            }
            else
            {
                saleO.BillingCompany = "Undefined";
                saleO.DeliveryCompany = "Undefined";
                saleO.Company =  "Individual User Account";
            }

            if (order.Address != null)
            {
                saleO.BillingAddress1 = order.Address.Address1;
                saleO.BillingAddress2 = order.Address.Address2;
                saleO.BillingCity = order.Address.City;
                saleO.BillingPostalCode = order.Address.PostCode;
                saleO.DeliveryAddress1 = order.Address.Address1;
                saleO.DeliveryAddress2 = order.Address.Address2;
                saleO.DeliveryCity = order.Address.City;
                saleO.DeliveryPostalCode = order.Address.PostCode;
            }
            else
            {
                saleO.BillingAddress1 = "Undefined";
                saleO.BillingAddress2 = "Undefined";
                saleO.BillingCity = "Undefined";
                saleO.BillingPostalCode = "Undefined";
                saleO.DeliveryAddress1 = "Undefined";
                saleO.DeliveryAddress2 = "Undefined";
                saleO.DeliveryCity = "Undefined";
                saleO.DeliveryPostalCode = "Undefined";
            }

            if (order.CostCenter != null)
            {
                saleO.MemberCostCenter = order.CostCenter.Name;
            }
            else if (order.Location != null)
            {
                saleO.MemberCostCenter = order.Location.Name;
            }
            else
            {
                saleO.MemberCostCenter = "Undefined";
            }

            if (order.User != null)
            {
                saleO.FirstName = order.User.FirstName;
                saleO.LastName = order.User.LastName;
                saleO.Email = order.User.Email;
                saleO.Phone = order.User.PhoneNumber;
                saleO.MemberEmail = order.User.Email;
            }
            else
            {
                saleO.FirstName = "Undefined";
                saleO.LastName = "Undefined";
                saleO.Email = "Undefined";
                saleO.Phone = "Undefined";
                saleO.MemberEmail = "Undefined";
            }

            if (string.IsNullOrEmpty(order.OrderRef))
            {
                saleO.CustomerOrderNo = "Undefined";

            }
            else
            {
                saleO.CustomerOrderNo = order.OrderRef;
            }
            saleO.IsApproved = true;
            saleO.ProductTotal = order.OriginalSubtotal;
                saleO.DiscountTotal = Convert.ToDecimal(order.Discount);
                saleO.LineItems = getLineItems(GetProducts(order), order).ToArray();
                saleO.InternalComments = "";
            CreateResult result = api.SalesOrders.Create(saleO);
            return result;

        }

        public async Task<CreateResult> DoCin7ReqIndividual(string id)
        {
            //make connection to CIN7
            var api = new Cin7Api(new ApiUser("StationeryinMotionSA", "1251ada5191046238405f5a66ebbac8f"));

            //Get Order
            var order = _ctx.Orders
                 .Include(x => x.OrderProducts)

                 .Include(x => x.Account)
                    .ThenInclude(x => x.Locations)
                 .Include(x => x.Address)
                 .Include(x => x.User)
                 .FirstOrDefault(x => x.OrderNumber == id);

            if (order.Account == null)
            {
                order = _ctx.Orders
                 .Include(x => x.OrderProducts)
                 .Include(x => x.Address)
                 .Include(x => x.User)
                 .FirstOrDefault(x => x.OrderNumber == id);
            }

            var products = _ctx.Products;

            Cin7ApiWrapper.Models.SalesOrder saleO = new Cin7ApiWrapper.Models.SalesOrder();

            saleO.CreatedDate = order.Created;
            saleO.ModifiedDate = order.AuthorizedDate;
            saleO.Reference = order.OrderNumber;

            saleO.CurrencyCode = "ZAR";
            saleO.CurrencySymbol = "R";
            saleO.TaxStatus = TaxStatus.Excl;
            saleO.TaxRate = 0.14m;
            saleO.Total = Convert.ToDecimal(order.SubTotal);

            if (order.Account != null)
            {
                saleO.BillingCompany = order.Account.CompanyName;
                saleO.DeliveryCompany = order.Account.CompanyName;
                saleO.Company = order.Account.CompanyName;
            }
            else
            {
                saleO.BillingCompany = "Individual";
                saleO.DeliveryCompany = "Individual";
                saleO.Company = "Individual";
            }

            if (order.Address != null)
            {
                saleO.BillingAddress1 = order.Address.Address1;
                saleO.BillingAddress2 = order.Address.Address2;
                saleO.BillingCity = order.Address.City;
                saleO.BillingPostalCode = order.Address.PostCode;
                saleO.DeliveryAddress1 = order.Address.Address1;
                saleO.DeliveryAddress2 = order.Address.Address2;
                saleO.DeliveryCity = order.Address.City;
                saleO.DeliveryPostalCode = order.Address.PostCode;
            }
            else
            {
                saleO.BillingAddress1 = "Undefined";
                saleO.BillingAddress2 = "Undefined";
                saleO.BillingCity = "Undefined";
                saleO.BillingPostalCode = "Undefined";
                saleO.DeliveryAddress1 = "Undefined";
                saleO.DeliveryAddress2 = "Undefined";
                saleO.DeliveryCity = "Undefined";
                saleO.DeliveryPostalCode = "Undefined";
            }

            if (order.CostCenter != null)
            {
                saleO.MemberCostCenter = order.CostCenter.Name;
            }
            else if (order.Location != null)
            {
                saleO.MemberCostCenter = order.Location.Name;
            }
            else
            {
                saleO.MemberCostCenter = "Individual";
            }

            if (order.User != null)
            {
                saleO.FirstName = order.User.FirstName;
                saleO.LastName = order.User.LastName;
                saleO.Email = order.User.Email;
                saleO.Phone = order.User.PhoneNumber;
                saleO.MemberEmail = order.User.Email;
            }
            else
            {
                saleO.FirstName = "Undefined";
                saleO.LastName = "Undefined";
                saleO.Email = "Undefined";
                saleO.Phone = "Undefined";
                saleO.MemberEmail = "Undefined";
            }

            if (string.IsNullOrEmpty(order.OrderRef))
            {
                saleO.CustomerOrderNo = "Undefined";

            }
            else
            {
                saleO.CustomerOrderNo = order.OrderRef;
            }
            saleO.IsApproved = true;
            saleO.ProductTotal = order.OriginalSubtotal;
            saleO.DiscountTotal = Convert.ToDecimal(order.Discount);
            saleO.LineItems = getLineItems(GetProducts(order), order).ToArray();
            saleO.InternalComments = "";
            CreateResult result = api.SalesOrders.Create(saleO);
            return result;

        }
        public List<SalesOrderLineitem> getLineItems(List<Shop.Domain.Models.Products.Product> _products, Shop.Domain.Models.Orders.Order order)
        {
            List<SalesOrderLineitem> li = new List<SalesOrderLineitem>();

            foreach (var x in _products)
            {
                SalesOrderLineitem soli = new SalesOrderLineitem
                {
                    Id = 1,
                    CreatedDate = DateTime.UtcNow,
                    TransactionId = 3,
                    ProductId = 1,
                    ProductOptionId = 6,
                    StyleCode = x.Colour,
                    Code = x.ExternalId,
                    Name = x.Name,
                    Option1 = x.Unit,
                    Option2 = x.Brand,
                    SizeCodes = "none",
                    LineComments = "none",
                    IntegrationRef = x.Id,
                    //Qty = x.order.Qty,

                    UnitPrice = x.Price ?? 0,
                    Discount = 0,
                    QtyShipped = 0
                };
                foreach (var p in order.OrderProducts)
                {
                    if (x.Id == p.ProductId)
                        soli.Qty = p.Qty;
                }
                li.Add(soli);
            }
            return li;
        }

        //public IQueryable<Shop.Domain.Models.Products.Product> FilterProducts(Shop.Domain.Models.Orders.Order order)
        //{
        //    List<string> productsOfSalesOrderIds = new List<string>();

        //    foreach (var product in order.OrderProducts)
        //    {
        //        productsOfSalesOrderIds.Add(product.ProductId);
        //    }
        //    var products = _ctx.Products;//.Where(x => order.OrderProducts.Contains(order.OrderProducts));

        //    //filter here for the products in the salesorder

        //    //var orderproducts = 
        //    return products;
        //}

        public List<Shop.Domain.Models.Products.Product> GetProducts(Shop.Domain.Models.Orders.Order order)
        {

            List<string> productsOfSalesOrderIds = new List<string>();

            foreach (var product in order.OrderProducts)
            {
                productsOfSalesOrderIds.Add(product.ProductId);
            }

            var ids = productsOfSalesOrderIds.ToArray();

            return _ctx.Products
                .Where(p => ids.Contains(p.Id))
                .ToList();

        }

        

       
    }
}
