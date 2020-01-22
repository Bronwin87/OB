using System;
using System.Collections.Generic;
using System.Text;
using Shop.Database;
using Shop.Domain;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.Models.Cart;
using System.Linq;

namespace Shop.Application.Services.Discounts
{
    public class Discounts
    {
        private ApplicationDbContext _ctx;
        public Discounts(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        

    }
}
