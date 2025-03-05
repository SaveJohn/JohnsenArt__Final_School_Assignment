using JoArtClassLib;
using Microsoft.EntityFrameworkCore;

namespace JoArtDataLayer;

public class JoArtDbContext : DbContext
{
    public JoArtDbContext(DbContextOptions<JoArtDbContext> options) : base(options)
    {
    }

    public DbSet<Listing> Listings { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Listing)
            .WithOne(l => l.Order)
            .HasForeignKey<Order>(o => o.ListingId);
    }
}