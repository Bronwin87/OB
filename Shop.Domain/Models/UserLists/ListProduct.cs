using System;
using System.Collections.Generic;
using System.Text;
using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.UserLists
{
    public class ListProduct
    {
        public int Id { get; set; }
        public UserLists Userlist { get; set; }
        
        public string ProductId { get; set; }
        public Product Product { get; set; }
        public int Qty { get; set; }
    }
}
