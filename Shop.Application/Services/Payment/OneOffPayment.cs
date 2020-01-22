using Microsoft.Extensions.Options;
using PayFast;
using System;
using System.Text;
using System.Web;

namespace Shop.Application.Payment
{
    public class OneOffPayment
    {
        private PayFastSettings _payFastSettings;

        public OneOffPayment(
            IOptions<PayFastSettings> payFastOptions)
        {
            _payFastSettings = payFastOptions.Value;
        }

        public class Request
        {
            public string PaymentReference { get; set; }
            public decimal TotalValue { get; set; }
            public string BuyerEmail { get; set; }
        }

        public string GetRedirectLink(Request request)
        {

            //var onceOffRequest = new PayFastRequest()
            //{
            //    // Merchant Details
            //    merchant_id = _payFastSettings.MerchantId,
            //    merchant_key = _payFastSettings.MerchantKey,
            //    return_url = _payFastSettings.ReturnUrl,
            //    cancel_url = _payFastSettings.CancelUrl,
            //    notify_url = _payFastSettings.NotifyUrl, //+ "?OrderReference=" + request.PaymentReference,
            //     //notify_url = _payFastSettings.NotifyUrl + "?OrderReference=" + request.PaymentReference,
            //     name_first = "Bronwin",
            //     name_last = "Bergstedt",
            //    // Buyer Details
            //    email_address = request.BuyerEmail,

            //    // Transaction Details, need to change to real details
            //    m_payment_id = request.PaymentReference,
            //    amount = request.TotalValue,
            //    //for dev testing:
            //    //m_payment_id = "8d00bf49-e979-4004-228c-08d452b86380",
            //    //amount = 30,
            //    item_name = "OfficeBox (Pty) Ltd",
            //    item_description = "OfficeBox Stationery Order",

            //    // Transaction Options
            //    email_confirmation = true,
            //    confirmation_address = request.BuyerEmail
            //};
            //var redirectUrl = $"{_payFastSettings.ProcessUrl}{onceOffRequest.ToString()}";

            //BronwinHack for now

            StringBuilder str = new StringBuilder();

            str.Append("merchant_id=" + System.Web.HttpUtility.UrlEncode(_payFastSettings.MerchantId));
            str.Append("&merchant_key=" + HttpUtility.UrlEncode(_payFastSettings.MerchantKey));
            str.Append("&return_url=" + _payFastSettings.ReturnUrl);
            str.Append("&cancel_url=" + _payFastSettings.CancelUrl);
            str.Append("&notify_url=" + _payFastSettings.NotifyUrl);
            //
            str.Append("&name_first=" + HttpUtility.UrlEncode(""));
            str.Append("&name_last=" + HttpUtility.UrlEncode(""));
            //str.Append("&email_address=" + HttpUtility.UrlEncode(request.BuyerEmail));
            str.Append("&email_address=" + request.BuyerEmail);

            //

            str.Append("&m_payment_id=" + HttpUtility.UrlEncode(request.PaymentReference));
            str.Append("&amount=" + (Math.Round(request.TotalValue, 2, MidpointRounding.AwayFromZero).ToString().Replace(',', '.'))); //.ToString("N2"));//.Replace(",","."));
            str.Append("&item_name=" + HttpUtility.UrlEncode("OfficeBox (Pty) Ltd"));
            str.Append("&item_description=" + HttpUtility.UrlEncode("OfficeBox Stationery Order"));
            ///////////////////////////////////////////////////////////////////////////////////////////
            ///
            str.Append("&email_confirmation=" + HttpUtility.UrlEncode("1"));
            str.Append("&confirmation_address=" + "orders@officebox.co.za");

            var redirectUrl2 = $"{_payFastSettings.ProcessUrl}{str}";
            return redirectUrl2;
            //return redirectUrl;
        }

    }
}
