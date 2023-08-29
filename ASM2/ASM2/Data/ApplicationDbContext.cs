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
        /*this config is to enable lazy loading, the reason why is use this in the project is because for
         whatever reason eager loading refuse to retun the product variables so i have to collect it using
        lazy loading.
        *note this verson of lazy loading require an extention
        extention download instruction link:https://www.nuget.org/packages/Microsoft.EntityFrameworkCore.Proxies/
        */
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLazyLoadingProxies(); // Enable lazy loading
        }*/
        public DbSet<ASM2.Models.Category> Category { get; set; } = default!;
        public DbSet<ASM2.Models.Product> Product { get; set; } = default!;
        public DbSet<ASM2.Models.Role> Role { get; set; } = default!;
        public DbSet<ASM2.Models.Cart> Cart { get; set; } = default!;
        public DbSet<ASM2.Models.CartItem> CartItem { get; set; } = default!;
	}
}
