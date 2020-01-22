using Shop.UI.Infrastructure;

namespace Shop.UI.Pages.UserProfile
{
    public class IndexModel : BasePage
    {
        public bool WelcomeMessage { get; set; }

        public void OnGet(bool welcome = false)
        {
            WelcomeMessage = welcome;
        }
    }
}