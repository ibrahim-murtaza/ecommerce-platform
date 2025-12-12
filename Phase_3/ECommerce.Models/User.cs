using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ECommerce.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [MaxLength(100)]
        public string Email { get; set; } = null!;

        [Required]
        public string PasswordHash { get; set; } = null!;

        [MaxLength(50)]
        public string FirstName { get; set; } = null!;

        [MaxLength(50)]
        public string LastName { get; set; } = null!;

        public string Address { get; set; } = null!;

        [MaxLength(50)]
        public string City { get; set; } = null!;

        public virtual ICollection<Order> Orders { get; set; } = null!;
        public virtual ICollection<Cart> Carts { get; set; } = null!;
    }
}