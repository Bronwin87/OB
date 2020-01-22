using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Domain.Models
{
    public class Discounts
    {
        public decimal CartTotal { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Threshold { get; set; }
        public decimal Percentage { get; set; }
    }
}
