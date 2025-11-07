using API_CAPITAL_MANAGEMENT.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Data
{
    public class MyAppDbContext : DbContext
    {
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) :base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Movement> Movements { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ===== User - Employee =====
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.User)
                .WithMany() // o .WithMany(u => u.Employees) si tienes la colección
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 🔹 evita cascada

            // ===== Employee - Organization =====
            modelBuilder.Entity<Employee>()
                .HasOne(e => e.Organization)
                .WithMany() // o .WithMany(o => o.Employees)
                .HasForeignKey(e => e.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict); // 🔹 evita cascada

            // ===== Organization - User =====
            modelBuilder.Entity<Organization>()
                .HasOne(o => o.User)
                .WithMany() // o .WithMany(u => u.Organizations)
                .HasForeignKey(o => o.UserId)
                .OnDelete(DeleteBehavior.Restrict); // 🔹 evita cascada

            // ===== Movement - Organization =====
            modelBuilder.Entity<Movement>()
                .HasOne(m => m.Organization)
                .WithMany() // o .WithMany(o => o.Movements)
                .HasForeignKey(m => m.OrganizationId)
                .OnDelete(DeleteBehavior.Restrict); // 🔹 evita cascada
        }
    }
}
