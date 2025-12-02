using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        // Composite Key (OrderItemID + OrderDate) is configured in DbContext, not here via attributes
        public int OrderItemID { get; set; }
        
        public int OrderID { get; set; }
        
        public DateTime OrderDate { get; set; } // Part of partition key

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }

        // Navigation Properties
        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;
        
        [ForeignKey("OrderID, OrderDate")] // Composite Foreign Key
        public virtual Order Order { get; set; } = null!;
    }
}