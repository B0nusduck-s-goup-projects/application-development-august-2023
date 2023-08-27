using System.ComponentModel.DataAnnotations;

namespace ASM2.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(80)]
        public string Name { get; set; } = null!;
        public virtual ICollection<Product>? Products { get; set; }
    }
}
