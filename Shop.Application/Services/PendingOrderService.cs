using Shop.Application.Services.Quotes;
using Shop.Database;
using Shop.Domain.Enums;
using Shop.Domain.Models.PendingOrders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.Application.Services
{
    public class PendingOrderService
    {
        private ApplicationDbContext _ctx;
        private QuotesService _quotesService;

        public PendingOrderService(ApplicationDbContext ctx, QuotesService quotesService)
        {
            _ctx = ctx;
            _quotesService = quotesService;
        }

        public PendingOrder CreatePendingOrder(int cartId, string userId)
        {
            var request = new QuotesService.Request { UserId = userId };
            var quote = _quotesService.CreateQuote(request);
            if (quote == null)
            {
                return null;
            }

            return CreatePendingOrder(quote.Id, userId, string.Empty);
        }

        public PendingOrder CreatePendingOrder(int quoteId, string userId, string orderNumber)
        {
            var quote = _ctx.Quotes.FirstOrDefault(x => x.Id == quoteId);
            if (quote == null)
            {
                return null;
            }

            var user = _ctx.AccountUsers.FirstOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return null;
            }

            var pendingOrder = new PendingOrder
            {
                Created = DateTime.UtcNow,
                AccountId = user.AccountId,
                QuoteId = quoteId,
                OrderNumber = orderNumber,
                UserId = userId
            };

            foreach (var product in quote.QuoteProducts)
            {
                pendingOrder.PendingOrderProducts.Add(new PendingOrderProduct()
                {
                    ProductId = product.ProductId,
                    Qty = product.Qty
                });
            }

            _ctx.PendingOrders.Add(pendingOrder);
            return pendingOrder;
        }

        public void RemoveItemFromPendingOrder(int pendingOrderId, string productId)
        {
            var pendingOrder = _ctx.PendingOrders.FirstOrDefault(x => x.Id == pendingOrderId);

            var product = pendingOrder?.PendingOrderProducts.FirstOrDefault(x => x.ProductId == productId);
            if (product == null)
            {
                return;
            }

            pendingOrder.PendingOrderProducts.Remove(product);

            _ctx.SaveChanges();
        }

        public void AdjustPendingOrderProductQuantity(int pendingOrderId, string productId, int newQuantity)
        {
            var pendingOrderProduct = _ctx.PendingOrderProducts.FirstOrDefault(x => x.PendingOrderId == pendingOrderId && x.ProductId == productId);
            if (pendingOrderProduct != null)
            {
                pendingOrderProduct.Qty = newQuantity;
            }

            _ctx.SaveChanges();
        }

        public IEnumerable<PendingOrder> GetQuotes(string userId)
        {
            var user = _ctx.AccountUsers.FirstOrDefault(x => x.UserId == userId);
            if (user == null)
            {
                return Array.Empty<PendingOrder>();
            }

            // If the user is superUser or Authorizer, then we serve all the pending orders for the account.
            if (user.User.Type == UserType.SuperUser || user.User.Type == UserType.Authorizer)
            {
                return user.Account.PendingOrders;
            }

            // else if the normal user, then serve pending orders for the user.
            return user.User.PendingOrders;
        }
    }
}
