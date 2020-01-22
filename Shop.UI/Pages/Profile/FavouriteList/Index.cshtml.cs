using Microsoft.AspNetCore.Mvc;
using Shop.Application.Services.FavouriteLists;
using Shop.UI.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shop.UI.Pages.Profile.FavouriteList
{
    public class IndexModel : BasePage
    {
        private readonly GetListsFull _getListsFull;
        private readonly DeleteList _deleteList;

        public IndexModel(GetListsFull getListsFull, DeleteList deleteList)
        {
            this._getListsFull = getListsFull;
            this._deleteList = deleteList;
        }

        [BindProperty]
        public int DeleteListId { get; set; }

        public List<FavouriteListItemModel> List { get; set; }

        public void OnGet()
        {
            var currentUserList = _getListsFull.Do(GetUserId());

            List = currentUserList.Select(ul => new FavouriteListItemModel()
            {
                Id = ul.Id,
                Name = ul.Name,
                TotalQty = ul.TotalQty,
                TotalPrice = ul.TotalPrice
            }).ToList();
        }

        public async Task<IActionResult> OnPost()
        {
            await _deleteList.Do(DeleteListId);
            return RedirectToPage("/Profile/FavouriteList/Index");
        }

        public class FavouriteListItemModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string TotalQty { get; set; }
            public string TotalPrice { get; set; }
        }
    }
}