using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("OrderItem")]
    public class OrderItem
    {
        public int OrderItemID { get; set; }
        
        public int OrderID { get; set; }
        
        public DateTime OrderDate { get; set; } 

        public int ProductID { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PriceAtPurchase { get; set; }

        [ForeignKey("ProductID")]
        public virtual Product Product { get; set; } = null!;
        
        [ForeignKey("OrderID, OrderDate")]
        public virtual Order Order { get; set; } = null!;
    }
}