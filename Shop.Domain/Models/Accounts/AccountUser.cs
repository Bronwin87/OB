using Shop.Domain.Models.Users;

namespace Shop.Domain.Models.Accounts
{
    public class AccountUser
    {
        public int AccountId { get; set; }
        public Account Account { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public bool Active { get; set; }
    }
}
