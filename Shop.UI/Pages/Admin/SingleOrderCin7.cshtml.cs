using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.UI.Pages.Admin
{
    public class SingleOrderCin7Model : PageModel
    {
        public Cin7OrderModel ViewModel { get; set; }

        public void OnGet(string orderNumber, string cin7Id)
        {
            ViewModel = new Cin7OrderModel()
            {
                OrderNumber = orderNumber,
                Cin7Id = cin7Id
            };
        }

        public class Cin7OrderModel
        {
            public string OrderNumber { get; set; }
            public string Cin7Id { get; set; }
            public string Cin7Url
            {
                get
                {
                    return "https://go.cin7.com/Cloud/TransactionEntry/TransactionEntry.aspx?idCustomerAppsLink=3293&OrderId=" + Cin7Id;
                }
            }
        }
    }
}