using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("Admin")]
    public class Admin
    {
        [Key]
        public int AdminID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [MaxLength(20)]
        public string Role { get; set; } = null!;

        public bool IsActive { get; set; }
    }
}