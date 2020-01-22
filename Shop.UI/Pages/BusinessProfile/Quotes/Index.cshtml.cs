using Shop.Application.Services.Quotes;
using Shop.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Shop.UI.Pages.BusinessProfile.Quotes
{
    public class IndexModel : BasePage
    {
        private readonly QuotesService _quotesService;

        public IndexModel(QuotesService quotesService)
        {
            this._quotesService = quotesService;
        }

        public List<QuoteViewModel> AllQuotes { get; set; }

        public void OnGet()
        {
            var quotes = _quotesService.GetQuotes(GetUserId());

            AllQuotes = quotes.Select(q => new QuoteViewModel()
            {
                Id = q.Id,
                Name = q.Name,
                DateCreated = q.DateCreated,
                Total = q.TotalPrice
            }).ToList();
        }

        public class QuoteViewModel
        {
            public int Id { get; set; }
            public string DateCreated { get; set; }
            public string Name { get; set; }
            public string Total { get; set; }
        }
    }
}