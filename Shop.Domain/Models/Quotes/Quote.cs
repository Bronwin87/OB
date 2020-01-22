using Shop.Domain.Models.Accounts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Shop.Domain.Models.Users;

namespace Shop.Domain.Models.Quotes
{
    public class Quote
    {
        public Quote()
        {
            QuoteProducts = new List<QuoteProduct>();
        }

        [Key]
        public int Id { get; set; }

        [NotMapped]
        public string QuoteNumber => string.Concat(Created.Date, Id);

        public string Name { get; set; }

        public  DateTime Created { get; set; }

        public int? AccountId { get; set; }
        public Account Account { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public ICollection<QuoteProduct> QuoteProducts { get; set; }
    }
}
