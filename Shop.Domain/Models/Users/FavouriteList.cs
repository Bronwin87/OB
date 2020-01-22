using System.Collections.Generic;

namespace Shop.Domain.Models.Users
{
    public class FavouriteList
    {
        public FavouriteList()
        {
            FavouriteProducts = new List<FavouriteListProduct>();
        }

        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<FavouriteListProduct> FavouriteProducts { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
