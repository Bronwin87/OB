using Shop.Application.Cart;
using Shop.Domain.Models.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Models
{
    public class CartViewModel
    {
        public CartViewModel()
        {
            this.CartItems = new List<GetCart.Response>();
            this.Account = new Account();
        }

        public string Name { get; set; }

        public decimal Discounts
        {
            get
            {
                return this.CartItems.FirstOrDefault().ProductThresholdDiscount;
            }
        }

        public decimal CartSubTotalExcl
        {
            get
            {
                return this.CartItems.Sum(ci => ci.Qty * ci.Value) - Discounts;
            }
        }

       

        public decimal ShippingTotalExcl
        {
            get
            {
                if (this.CartSubTotalExcl < 650)
                {
                    return 60m;
                }
                else
                {
                    return 0m;
                }
            }
        }

        public decimal CartTotalExcl
        {
            get
            {
                return CartSubTotalExcl + ShippingTotalExcl;
            }
        }

        public decimal CartVat
        {
            get
            {
                return CartTotalExcl * 0.15m;
            }
        }

        public decimal CartTotalIncl
        {
            get
            {
                return CartTotalExcl + CartVat;
            }
        }

        public string ShippingTotalString
        {
            get
            {
                if (this.CartSubTotalExcl < 650)
                {
                    return string.Format("R {0}", 60m.ToString("N2"));
                }
                else
                {
                    return "Free";
                }
            }
        }

        public string CartThresholdDiscount2
        {
            get
            {
                if (this.Discounts!= 0)
                {
                    return string.Format("R {0}", Discounts.ToString("N2"));
                }
                else
                {
                    return "0";
                }
            }
        }

        public string CartThresholdDiscountForQuote
        {
            get
            {
                if (this.Discounts != 0)
                {
                    return string.Format("R {0}", Discounts.ToString("N2"));
                }
                else
                {
                    return "0";
                }
            }
        }

        public Account Account { get; set; }
        public List<GetCart.Response> CartItems { get; set; }
    }
}