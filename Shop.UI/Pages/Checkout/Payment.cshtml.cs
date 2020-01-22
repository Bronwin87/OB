using Microsoft.AspNetCore.Mvc;
using Shop.Application.Oders;
using Shop.Application.Payment;
using Shop.Database;
using Shop.UI.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Checkout
{
    public class PaymentModel : BasePage
    {
        private ApplicationDbContext _ctx;
        private OneOffPayment _payment;
        private CreateOrder _createOrder;
        private GetOrder _getOrder;

        public PaymentModel(
            ApplicationDbContext ctx,
            CreateOrder createOrder,
            GetOrder getOrder,
            OneOffPayment payment)
        {
            _ctx = ctx;
            _payment = payment;
            _createOrder = createOrder;
            _getOrder = getOrder;
        }

        public async Task<IActionResult> OnGet(string orderNumber)
        {
            //todo perform some checks here maybe?
            //not yet at least

            if (string.IsNullOrEmpty(orderNumber))
            {
                return await RedirectToPayment();
            }
            return await ReturnToPayment(orderNumber);
        }

        private async Task<IActionResult> RedirectToPayment()
        {
            var userType = GetUserType();
            var isBusiness = userType != "user";

            var PaymentReference = Guid.NewGuid().ToString();

            var mark = GetCartUserMark();

            int? accountId = null;

            if (isBusiness)
                accountId = _ctx.AccountUsers.FirstOrDefault(x => x.UserId == mark.userId && x.Active).AccountId;

            var order = await _createOrder.Do(new CreateOrder.Request
            {
                UserMark = mark,
                PayementReference = PaymentReference,
                AccountId = accountId
            });

            var redirectUrl = _payment.GetRedirectLink(new OneOffPayment.Request
            {
                BuyerEmail = GetUserEmail(),
                //PaymentReference = PaymentReference,
                PaymentReference = order.OrderNumber,
                TotalValue = order.SubTotal
            });

            return Redirect(redirectUrl);
        }

        private async Task<IActionResult> ReturnToPayment(string orderNumber)
        {
            var userType = GetUserType();
            var isBusiness = userType != "user";

            var PaymentReference = Guid.NewGuid().ToString();

            var order = _getOrder.Do(orderNumber);

            var redirectUrl = _payment.GetRedirectLink(new OneOffPayment.Request
            {
                BuyerEmail = GetUserEmail(),
                //PaymentReference = PaymentReference,
                PaymentReference = order.OrderNumber,
                TotalValue = order.Subtotal
            });

            return Redirect(redirectUrl);
        }
    }
}