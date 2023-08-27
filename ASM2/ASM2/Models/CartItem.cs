using System.ComponentModel.DataAnnotations;

namespace ASM2.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public int CartId { get; set; }
        public Cart Cart = null!;
        [Required]
        public int ProductId { get; set; }
        public string? ProductName { get; set; }
        public Product Product = null!;
        [Required]
        public int Quantity { get; set; }
        [Required]
        public int TotalPrice { get; set;}
    }
}
