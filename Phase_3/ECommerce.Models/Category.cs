using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("Category")]
    public class Category
    {
        [Key]
        public int CategoryID { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; } = null!;

        public string Description { get; set; } = null!;
        public bool IsActive { get; set; }

        public virtual ICollection<Product> Products { get; set; } = null!;
    }
}