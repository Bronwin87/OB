using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shop.Application.Services.FavouriteLists;
using Shop.UI.Infrastructure;
using Shop.Application.Services.Products.Entities;
using Shop.Database;

namespace Shop.UI.Pages.BusinessProfile.FavouriteList
{
    public class DetailModel : BasePage
    {
        private readonly GetListsFull _getListsFull;
        private readonly UpdateList _updateList;

        public DetailModel(GetListsFull getListsFull, UpdateList updateList)
        {
            this._getListsFull = getListsFull;
            this._updateList = updateList;
        }

        [BindProperty]
        public FavouriteListItemModel ListModel { get; set; }

        public void OnGet(int id)
        {
            var currentUserList = _getListsFull.Do(GetUserId());
            this.ListModel = currentUserList.Where(c => c.Id == id)
                                       .Select(ul => new FavouriteListItemModel()
                                       {
                                           Id = ul.Id,
                                           Name = ul.Name,
                                           TotalQty = ul.TotalQty,
                                           TotalPrice = ul.TotalPriceDec,
                                           Products = ul.Products.Select(lp => new FavouriteListProductViewModel()
                                           {
                                               Id = lp.Id,
                                               ImageUrl = lp.ImageUrl,
                                               Name = lp.Name,
                                               Code = lp.Code,
                                               Uom = lp.Uom,
                                               Price = lp.Price,
                                               //Qty = lp.Qty,
                                               Qty = 1,
                                               TotalPrice = lp.TotalPrice
                                           }).ToList()
                                       }).FirstOrDefault();
        }

        public async Task<IActionResult> OnPost(string SubmitAction)
        {
            UpdateList.Request request = new UpdateList.Request()
            {
                Id = ListModel.Id,
                Name = ListModel.Name,
                Products = ListModel.Products.Select(p => new UpdateList.RequestProduct()
                {
                    Id = p.Id,
                    Qty = p.Qty,
                    Deleted = p.Deleted
                })
            };

            await _updateList.Do(request);

            if (SubmitAction == "AddCart")
            {
                return RedirectToAction("Cart", "FavList", new { id = ListModel.Id, redirectToCheckout = true });
            }

            return RedirectToPage("/BusinessProfile/FavouriteList/Detail", new { id = ListModel.Id });
        }
    }

    public class FavouriteListItemModel
    {
        public FavouriteListItemModel()
        {
            this.Products = new List<FavouriteListProductViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string TotalQty { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Delivery
        {
            get
            {
                return this.Products.Any() ? (this.TotalPrice > 650m ? 0m : 60.00m) : 0m;
            }
        }
        public string DeliveryText
        {
            get
            {
                return this.Delivery == 0 ? "Free" : "R " + this.Delivery.ToString("N2");
            }
        }
        public decimal SubTotal
        {
            get
            {
                return this.TotalPrice + this.Delivery;
            }
        }
        public decimal Vat
        {
            get
            {
                return SubTotal * 0.15m;
            }
        }
        public decimal TotalPriceIncl
        {
            get
            {
                return SubTotal + Vat;
            }
        }
        public List<FavouriteListProductViewModel> Products { get; set; }
    }
}