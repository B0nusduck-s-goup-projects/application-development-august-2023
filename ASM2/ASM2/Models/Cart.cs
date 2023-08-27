using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ASM2.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(450)]
        public string UserId { get; set; } = null!;
        public IdentityUser User = null!;
        public bool IsDeleted { get; set; } = false;
        public ICollection<CartItem>? CartItem { get; set; }
	}
}
