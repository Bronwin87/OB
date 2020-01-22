using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile.Term
{
    public class IndexModel : PageModel
    {

        public bool Confirm { get; set; }

        public void OnGet(bool confirm)
        {
            Confirm = confirm;
        }

        public async Task<IActionResult> OnPost()
        {

            //do stuff here

            return RedirectToPage("/BusinessProfile/Term/Index", new { confirm = true });
        }
    }
    public class TermUpdateViewModel
    {
        public int CreditLimit { get; set; }
    }
}