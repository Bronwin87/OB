using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.UI.Pages.Checkout
{
    public class PayfastConfirmation : PageModel
    {
        public bool OrderPlaced { get; private set; }

        public string OrderReferenceModel { get; set; }

        public IActionResult OnGet(string OrderReference, bool placed = false)
        {
            //handle payment confirmation.
            OrderPlaced = placed;
            OrderReferenceModel = OrderReference;
            return Page();
        }
    }
}