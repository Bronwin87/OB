using Shop.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shop.Domain.Models.Products
{
    public class ProductLink
    {
        public string RootId { get; set; }
        [ForeignKey("RootId")]
        public Product Root { get; set; }

        public string TargetId { get; set; }
        [ForeignKey("TargetId")]
        public Product Target { get; set; }

        public ProductLinkType Type { get; set; }
    }
}
