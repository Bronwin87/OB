using Shop.Domain.Models.Products;

namespace Shop.Domain.Models.Users
{
    public class FavouriteListProduct
    {
        public int FavouriteListId { get; set; }
        public FavouriteList FavouriteList { get; set; }

        public string ProductId { get; set; }
        public Product Product { get; set; }

        public int Qty { get; set; }
    }
}
