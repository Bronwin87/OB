using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Shop.Domain.Models.Accounts;
using Shop.Domain.Models.Products;
using Shop.Domain.Models.Users;

namespace Shop.Domain.Models.PendingOrders
{
    public class PendingOrder
    {
        public PendingOrder()
        {
            PendingOrderProducts = new List<PendingOrderProduct>();
        }

        [Key]
        public int Id { get; set; }

        public DateTime Created { get; set; }

        public string OrderNumber { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? QuoteId { get; set; }

        public ICollection<PendingOrderProduct> PendingOrderProducts { get; set; }
    }
}
