using JoArtClassLib;
using JoArtClassLib.Art;
using Microsoft.EntityFrameworkCore;

namespace JoArtDataLayer;

public class JoArtDbContext : DbContext
{
    public JoArtDbContext(DbContextOptions<JoArtDbContext> options) : base(options)
    {
    }

    public DbSet<Artwork> Artworks { get; set; }
    
    public DbSet<ArtworkImage> ArtworkImages { get; set; }

    public DbSet<Admin> Admins { get; set; }

    public DbSet<Order> Orders { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>()
            .HasOne(o => o.Artwork)
            .WithOne(a => a.Order)
            .HasForeignKey<Order>(o => o.ArtworkId);
        
        modelBuilder.Entity<ArtworkImage>()
            .HasOne(i => i.Artwork)
            .WithMany(a => a.Images)
            .HasForeignKey(i => i.ArtworkId);
    }
}