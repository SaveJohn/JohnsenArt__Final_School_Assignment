using JoArtClassLib;
using Microsoft.EntityFrameworkCore;

namespace JoArtDataLayer;

public class JoArtDbContext : DbContext
{
    public JoArtDbContext(DbContextOptions<JoArtDbContext> options) : base(options)
    {
    }

    public DbSet<Artwork> Listings { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Artwork)
            .WithOne(a => a.Order)
            .HasForeignKey<Order>(o => o.ArtworkId);
    }
}