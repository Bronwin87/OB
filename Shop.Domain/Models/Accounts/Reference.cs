namespace Shop.Domain.Models.Accounts
{
    public class Reference
    {
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string ContactName { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; }

        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
