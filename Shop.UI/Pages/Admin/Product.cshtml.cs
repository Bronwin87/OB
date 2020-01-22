using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Shop.UI.Pages.Admin
{
    public class ProductModel : PageModel
    {
        public string Id { get; private set; }

        public void OnGet(string id)
        {
            Id = id;
        }
    }
}