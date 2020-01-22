using Microsoft.EntityFrameworkCore;
using Shop.Application.Infrastructure;
using Shop.Database;
using Shop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.OrdersAdmin
{
    public class GetOrders
    {
        private ApplicationDbContext _ctx;

        public GetOrders(ApplicationDbContext ctx)
        {
            _ctx = ctx;
            _ctx.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public class Request : PagedRequest
        {
            public string Search { get; set; }
        }

        public class Response : PagedRequest
        {
            public IEnumerable<OrderViewModel> Orders { get; set; }
        }

        public Response Do(Request request) =>
            new Response
            {
                Orders = _ctx.Orders
                    .Include(o => o.Account)
                    .Include(o => o.User)
                    .Include(o => o.OrderProducts)
                    .ThenInclude(o => o.Product)
                    .Where(x => EF.Functions.Like(x.OrderRef, $"%{request.Search}%"))
                    .Skip(request.PerPage * (request.Page - 1))
                    .Take(request.PerPage)
                    .Select(x => new OrderViewModel
                    {
                        OrderNumber = x.OrderNumber,
                        OrderRef = x.OrderRef,
                        Created = x.Created,
                        PaymentReference = x.PaymentReference,
                        Status = x.Status == OrderStatus.Complete
                            ? "Complete"
                            : x.Status == OrderStatus.PendingApproval || x.Status == OrderStatus.PendingPayment
                                ? "Pending"
                                : "Declined",
                        Total = x.OrderProducts.Sum(y => y.Qty * y.Product.Price ?? 0),
                        User = new UserViewModel
                        {
                            FirstName = x.User.FirstName,
                            LastName = x.User.LastName,
                            Username = x.User.UserName,
                            PhoneNumber = x.User.PhoneNumber,
                            Email = x.User.Email
                        },
                        Address = new AddressViewModel
                        {
                            Address1 = x.Address.Address1,
                            Address2 = x.Address.Address2,
                            City = x.Address.City,
                            Country = x.Address.Country,
                            PostCode = x.Address.PostCode,
                        },
                        Account = x.AccountId != null
                            ? new AccountViewModel { }
                            : null,
                        Products = x.OrderProducts.Select(y => new ProductViewModel
                        {
                            Name = y.Product.Name,
                            Code = y.Product.ExternalId,
                            Uom = y.Product.Unit,
                            Price = y.Product.Price ?? 0,
                            Qty = y.Qty,
                            Total = y.Product.Price ?? 0 * y.Qty,
                        })

                    })
                    .ToList(),
                Total = _ctx.Orders
                    .Where(x => EF.Functions.Like(x.OrderRef, $"%{request.Search}%"))
                    .Count()
            };

        public class OrderViewModel
        {
            public string OrderNumber { get; set; }
            public string OrderRef { get; set; }
            public string PaymentReference { get; set; }
            public DateTime Created { get; set; }

            public string Status { get; set; }
            public decimal Total { get; set; }

            public UserViewModel User { get; set; }
            public AccountViewModel Account { get; set; }
            public AddressViewModel Address { get; set; }
            public IEnumerable<ProductViewModel> Products { get; set; }
        }

        public class UserViewModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Username { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

        public class AccountViewModel
        {
        }

        public class AddressViewModel
        {
            public string Address1 { get; set; }
            public string Address2 { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string PostCode { get; set; }
        }

        public class ProductViewModel
        {
            public string Name { get; set; }
            public string Code { get; set; }
            public string Uom { get; set; }
            public decimal Price { get; set; }
            public int Qty { get; set; }
            public decimal Total { get; set; }
        }
    }
}
