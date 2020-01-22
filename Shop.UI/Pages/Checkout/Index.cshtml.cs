using Microsoft.AspNetCore.Mvc;
using Shop.Application.Cart;
using Shop.Application.Services.Cart;
using Shop.Database;
using Shop.UI.Infrastructure;
using System.Collections.Generic;
using Shop.Domain.Models;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using Shop.Domain.Models.Accounts;

namespace Shop.UI.Pages.Checkout
{
    public class IndexModel : BasePage
    {
        private ApplicationDbContext _ctx;
        private IOptions<Discounts> _discounts;

        public IndexModel(ApplicationDbContext ctx, IOptions<Discounts> discounts)
        {
            _ctx = ctx;
            _discounts = discounts;
        }

        [BindProperty]
        public AddToCart.Request Input { get; set; }
        public IEnumerable<GetCart.Response> Cart { get; set; }
        public double ThresholdDiscount { get; set; }
        //public Account account { get; set; }
        //public bool is30dayAccount { get; set; }
        
       

        public IActionResult OnGet()
        {   
            Cart = new GetCart(_ctx,_discounts).Do(GetCartUserMark());

            //var user = GetUserId();
            //var accountId = _ctx.AccountUsers.FirstOrDefault(x => x.UserId == user);
            //account = _ctx.Accounts.FirstOrDefault(x => x.Id == accountId.AccountId);
            //is30dayAccount = account.ThirtyDayTermApproved;

            return Page();
        }
    }
}