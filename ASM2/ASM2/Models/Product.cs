using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM2.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; } = null!;
        [Required]
        public int CategoryId { get; set; }
        [StringLength(80)]
        public string? CategoryName { get; set; }
        public Category Category = null!;
        [Required]
        public int Price { get; set; }
        public string? Image { get; set; }
        [StringLength(200)]
        public string? Description { get; set; }
        public List<CartItem>? CartItem { get; set;}
    }
}
