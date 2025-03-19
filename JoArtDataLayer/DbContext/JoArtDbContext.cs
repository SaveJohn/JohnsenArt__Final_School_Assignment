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


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        // Configure the primary key for Artwork
        modelBuilder.Entity<Artwork>()
            .HasKey(a => a.Id); // Id is the primary key

        // Configure the primary key for ArtworkImage
        modelBuilder.Entity<ArtworkImage>()
            .HasKey(ai => ai.Id); // Id is the primary key

        // Define the relationship between Artwork and ArtworkImage
        modelBuilder.Entity<ArtworkImage>()
            .HasOne(ai => ai.Artwork)
            .WithMany(a => a.Images)
            .HasForeignKey(ai => ai.ArtworkId); // FK for ArtworkImage references Artwork
    }
}