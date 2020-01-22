using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.UI.Pages.Checkout
{
    public class ConfirmationModel : PageModel
    {
        public bool OrderPlaced { get; private set; }

        public IActionResult OnGet(bool placed = false)
        {
            //handle payment confirmation.
            OrderPlaced = placed;
            return Page();
        }
    }
}