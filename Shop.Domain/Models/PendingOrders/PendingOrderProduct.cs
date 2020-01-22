using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.PendingOrders
{
    public class PendingOrderProduct
    {
        [Key]
        public int Id { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int Qty { get; set; }

        public int PendingOrderId { get; set; }
        public PendingOrder PendingOrder { get; set; }
    }
}
