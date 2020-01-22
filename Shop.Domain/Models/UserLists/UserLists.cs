using System;
using System.Collections.Generic;
using System.Text;
using Shop.Domain.Models.Users;
using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.UserLists
{
   
    public class UserLists
    {
        public UserLists()
        {
            ListProducts = new List<ListProduct>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public ICollection<ListProduct> ListProducts { get; set; }
    }
}
