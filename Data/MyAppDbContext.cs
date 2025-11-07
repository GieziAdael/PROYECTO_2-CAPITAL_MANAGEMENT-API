using API_CAPITAL_MANAGEMENT.Entities;
using Microsoft.EntityFrameworkCore;

namespace API_CAPITAL_MANAGEMENT.Data
{
    /// <summary>
    /// Database context for MyApp, managing entities and relationships.
    /// </summary>
    public class MyAppDbContext : DbContext
    {
        /// <summary>
        /// Constructor for MyAppDbContext
        /// </summary>
        /// <param name="options"></param>
        public MyAppDbContext(DbContextOptions<MyAppDbContext> options) :base(options)
        {
            
        }

        /// <summary>
        /// Entity set for Users.
        /// </summary>
        public DbSet<User> Users { get; set; }
        /// <summary>
        /// Entity set for Organizations.
        /// </summary>
        public DbSet<Organization> Organizations { get; set; }
        /// <summary>
        /// Entity set for Employees.
        /// </summary>
        public DbSet<Employee> Employees { get; set; }
        /// <summary>
        /// Entity set for Movements.
        /// </summary>
        public DbSet<Movement> Movements { get; set; }

        /// <summary>
        /// Provides configuration for the entity relationships and constraints.
        /// </summary>
        /// <param name="modelBuilder"></param>
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
