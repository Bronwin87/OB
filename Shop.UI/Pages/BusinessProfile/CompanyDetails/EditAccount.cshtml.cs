using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Shop.Database;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Shop.UI.Pages.BusinessProfile.CompanyDetails
{
    public class EditAccountModel : PageModel
    {
        private ApplicationDbContext _ctx;

        public EditAccountModel(ApplicationDbContext ctx)
        {
            _ctx = ctx;
        }

        [BindProperty]
        public AccountChangeViewModel Input { get; set; }

        public void OnGet()
        {
            var account = _ctx.AccountUsers
                .Include(x => x.Account)
                .ThenInclude(x => x.Address)
                .FirstOrDefault(x => x.UserId == User.FindFirst(ClaimTypes.NameIdentifier).Value && x.Active)
                .Account;

            Input = new AccountChangeViewModel
            {
                Id = account.Id,
                CompanyName = account.CompanyName,
                RegistrationNumber = account.RegistrationNumber,
                CompanyVAT = account.VatNumber,
                PhoneNumber = account.PhoneNumber,
                Address1 = account.Address.Address1,
                Address2 = account.Address.Address2,
                City = account.Address.City,
                PostCode = account.Address.PostCode
            };
        }

        public async Task<IActionResult> OnPost()
        {
            if (!ModelState.IsValid)
                return Page();

            var account = _ctx.Accounts
                .Include(x => x.Address)
                .FirstOrDefault(x => x.Id == Input.Id);

            account.PhoneNumber = Input.PhoneNumber;
            account.Address.Address1 = Input.Address1;
            account.Address.Address2 = Input.Address2;
            account.Address.City = Input.City;
            account.Address.PostCode = Input.PostCode;

            await _ctx.SaveChangesAsync();

            return RedirectToPage("/BusinessProfile/CompanyDetails/Index", new { message = "Account Updated" });
        }
    }
    public class AccountChangeViewModel
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string RegistrationNumber { get; set; }
        public string CompanyVAT { get; set; }

        [Required]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; } = "SOUTH AFRICA";
        [Required]
        public string PostCode { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
    }

}