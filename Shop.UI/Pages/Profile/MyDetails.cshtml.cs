using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Database;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Profile
{
    public class MyDetailsModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public MyDetailsModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }
        public string Message { get; set; }

        [BindProperty]
        public ProfileViewModel Input { get; set; }

        public void OnGet(string message)
        {
            var user = _ctx.Users.FirstOrDefault(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);

            Input = new ProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                JobTitle = user.JobTitle
            };

            Message = message;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = _ctx.Users.FirstOrDefault(x => x.Id == User.FindFirst(ClaimTypes.NameIdentifier).Value);
            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.Email = Input.Email;
            user.JobTitle = Input.JobTitle;
            await _ctx.SaveChangesAsync();

            return RedirectToPage("/Profile/MyDetails", new { message = "Details Updated" });
        }
    }
    public class ProfileViewModel
    {
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string JobTitle { get; set; }
    }
}