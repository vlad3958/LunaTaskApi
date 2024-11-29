using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace LunaTaskApi
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Task> Tasks { get; set; }

        // підключаємось до сервера
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=database-1.chau4g8kk1z7.us-east-1.rds.amazonaws.com;Initial Catalog=database-1.chau4g8kk1z7.us-east-1.rds.amazonaws.com;User ID=admin;Password=11111111;Trust Server Certificate=True");
        }
        public AppDbContext(DbContextOptions<AppDbContext> options)
       : base(options)  
        {
        }

        // встановлюємо сутності та зв'язки 
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)  // тільки унікальні значення емейлу та юзернейму
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Task>()
                .HasOne(t => t.User)       // один до багатьох
                .WithMany(u => u.Tasks)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }

}
