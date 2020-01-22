namespace Shop.Application.Infrastructure
{
    public class PagedRequest
    {
        public int Page { get; set; }
        public int PerPage { get; set; }
        public int Total { get; set; }
    }
}
