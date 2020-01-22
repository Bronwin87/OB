namespace Shop.Domain.Models
{
    public class Address
    {
        public int Id { get; set; }
        public string Address1 { get; set; } = "undefined";
        public string Address2 { get; set; } = "undefined";
        public string City { get; set; } = "undefined";
        public string Country { get; set; } = "undefined";
        public string PostCode { get; set; } = "undefined";
    }
}
