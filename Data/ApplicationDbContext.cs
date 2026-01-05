using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore; 
using Microsoft.EntityFrameworkCore;
using TaskManagement.Models;

namespace TaskManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<Users, IdentityRole<int>, int>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaskItem> Tasks { get; set; }

        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     base.OnModelCreating(modelBuilder);

        //     modelBuilder.Entity<TaskItem>()
        //         .HasOne(t => t.AssignedUser)
        //         .WithMany(u => u.AssignedTasks)
        //         .HasForeignKey(t => t.AssignedToId)
        //         .OnDelete(DeleteBehavior.Restrict);
        // }
    }
}
