using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("Order")]
    public class Order
    {
        // ID is part of composite key defined in Context, not here
        public int OrderID { get; set; }
        
        // Partition Key
        public DateTime OrderDate { get; set; }

        public int UserID { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [MaxLength(50)]
        public string Status { get; set; }

        public string ShippingAddress { get; set; }

        // Navigation
        public virtual User User { get; set; }
        public virtual ICollection<OrderItem> OrderItems { get; set; }
    }
}