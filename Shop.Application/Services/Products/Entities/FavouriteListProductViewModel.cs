using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.Services.Products.Entities
{
    public class FavouriteListProductViewModel
    {
        public int Index { get; set; }
        public string Id { get; set; }

        public string ImageUrl { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Uom { get; set; }
        public string Price { get; set; }
        public int Qty { get; set; }
        public string TotalPrice { get; set; }

        public bool Deleted { get; set; }   
    }

    public class QuoteProductViewModel
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductCode { get; set; }
        public string ImageUrl { get; set; }
        public string Uom { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; }
        public decimal TotalPrice
        {
            get
            {
                return this.Qty * this.Price;
            }
        }

        public bool Deleted { get; set; }
        public int Index { get; set; }
    }
}
