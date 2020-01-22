using Microsoft.EntityFrameworkCore;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.Orders;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.Application.Oders
{
    public class CreateOrder
    {
        private ApplicationDbContext _ctx;

        public CreateOrder(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        public class Request
        {
            public (string userId, string sessionId) UserMark { get; set; }
            public string OrderReference { get; set; }
            public string PayementReference { get; set; }
            public int? AccountId { get; set; }
            public bool PreserveCart { get; set; }
            public bool Approved { get; set; }
            public bool Term { get; set; }
            public int? LocationId { get; set; }
            public int? CostCenterId { get; set; }
        }

        public async Task<Order> Do(Request request)
        {
            var cart = _ctx.Carts
               .Include(x => x.Products)
               .ThenInclude(x => x.Product)
               .FirstOrDefault(x => (x.UserId == request.UserMark.userId
                       && !string.IsNullOrEmpty(request.UserMark.userId))
                   || (x.SessionId == request.UserMark.sessionId));


            var oRef = string.IsNullOrEmpty(request.OrderReference)
                ? cart.OrderReference
                : request.OrderReference;

            var order = new Order
            {
                Cin7ID = 0,
                Created = DateTime.UtcNow,
                OrderNumber = CreateOrderNumber(),
                OrderRef = oRef,
                PaymentReference = request.PayementReference,

                AccountId = request.AccountId,
                UserId = request.UserMark.userId,
                AddressId = (int)cart.AddressId,

                LocationId = request.LocationId,
                CostCenterId = request.CostCenterId,

                Discount = Math.Round(cart.Discount, 2),
                OriginalSubtotal = Math.Round(cart.OriginalSubtotal, 2),
                Delivery = cart.Delivery,
                HasVoucher = cart.HasVoucher,
                ProductCount = cart.ProductCount,
                SubTotal = Math.Round(cart.SubTotal, 2),
                Vat = Math.Round(cart.Vat, 2),

                Status = request.Term
                    ? request.Approved
                        ? OrderStatus.Placed
                        : OrderStatus.PendingApproval
                    : OrderStatus.PendingPayment
            };

            foreach (var p in cart.Products)
            {
                order.OrderProducts.Add(new OrderProduct
                {
                    //todo get correct discount price
                    ProductId = p.ProductId,
                    Qty = p.Qty
                });
            }

            _ctx.Orders.Add(order);

            if (!request.PreserveCart)
            {
                // Delete all carts that are linked to the user/session. I don't got now clue how, 
                // but somehow users are ending up with multiple carts attached to their user. 
                var carts = _ctx.Carts.Where(x =>
                            (x.UserId == request.UserMark.userId && !string.IsNullOrEmpty(request.UserMark.userId))
                         || (x.SessionId == request.UserMark.sessionId)).ToList();

                foreach (var item in carts)
                {
                    _ctx.Carts.Remove(item);
                }
            }

            await _ctx.SaveChangesAsync();

            return order;
        }


        public string CreateOrderNumber()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var result = new char[12];
            var random = new Random();

            do
            {
                for (int i = 0; i < result.Length; i++)
                    result[i] = chars[random.Next(chars.Length)];
            } while (_ctx.Orders.Any(x => x.OrderRef == new string(result)));

            return new string(result);
        }
    }
}
