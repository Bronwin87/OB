using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Application.Services.FavouriteLists;
using Shop.UI.Infrastructure;

namespace Shop.UI.Pages
{
    public class AddFavouriteListModel : BasePage
    {
        [BindProperty]
        public string ListName { get; set; }

        [BindProperty]
        public string ProfileView { get; set; }

        public void OnGet()
        { }

        public async Task<IActionResult> OnPostCreate([FromServices] CreateList createList)
        {
            if (string.IsNullOrEmpty(ListName))
            {
                ModelState.AddModelError("ListName", "List name is required");
                return Page();
            }

            await createList.Do(new CreateList.Request
            {
                UserId = GetUserId(),
                Name = ListName
            });

            //todo: redirect to this list
            return RedirectToPage("/" + ProfileView + "/FavouriteList/Index");
        }
    }
}