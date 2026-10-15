using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CommuteAlert.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace CommuteAlert.Web.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{

    public DbSet<Watch> Watches { get; set; }
    public DbSet<Alert> Alerts { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Watch>(e =>
        {
            e.Property(w => w.UserId).HasMaxLength(450);
            e.HasOne<IdentityUser>().WithMany().HasForeignKey(w => w.UserId)
            .IsRequired().OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<Alert>(e =>
        {
            e.Property(a => a.Key).HasMaxLength(100);
            e.Property(a => a.Message).HasMaxLength(500);
            e.HasOne<Watch>().WithMany().HasForeignKey(a => a.WatchId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(a => new { a.WatchId, a.Key }).IsUnique();
        });
    }


}
