using Microsoft.EntityFrameworkCore;
using Services.Model;
using System.Net;

namespace Services.Repository
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions dbContextOptions)
            : base(dbContextOptions)
        {
        }
        public DbSet<RefreshToken> RefreshToken { get; set; }
        public DbSet<Account> Account { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<Bill> Bill { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.HasIndex(e => e.Username).IsUnique();
                entity.Property(e => e.FullName).IsRequired();
                entity.Property(e => e.Email).IsRequired();
            });
        }
    }
}
