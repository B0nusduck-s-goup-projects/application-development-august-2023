using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ASM2.Models;

namespace ASM2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<ASM2.Models.Category> Category { get; set; } = default!;
        public DbSet<ASM2.Models.Product> Product { get; set; } = default!;
        public DbSet<ASM2.Models.Role> Role { get; set; } = default!;

        public DbSet<ASM2.Models.Cart> Cart { get; set; } = default!;

        public DbSet<ASM2.Models.CartItem> CartItem { get; set; } = default!;
	}
}