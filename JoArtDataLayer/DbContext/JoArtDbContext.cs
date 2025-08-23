using JoArtClassLib;
using JoArtClassLib.About;
using Microsoft.EntityFrameworkCore;
using Image = JoArtClassLib.Art.Image;

namespace JoArtDataLayer;

public class JoArtDbContext : DbContext
{
    public JoArtDbContext(DbContextOptions<JoArtDbContext> options) : base(options)
    {
    }

    public DbSet<Artwork> Artworks { get; set; }
    
    public DbSet<Image> ArtworkImages { get; set; }

    public DbSet<Admin> Admins { get; set; }
    
    public DbSet<BioBlock?> BioBlocks { get; set; }
    public DbSet<BioImage> BioImages { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        // Configure the primary key for Artwork
        modelBuilder.Entity<Artwork>()
            .HasKey(a => a.Id); // Id is the primary key

        // Configure the primary key for ArtworkImage
        modelBuilder.Entity<Image>()
            .HasKey(ai => ai.Id); // Id is the primary key

        // Define the relationship between Artwork and ArtworkImage
        modelBuilder.Entity<Image>()
            .HasOne(ai => ai.Artwork)
            .WithMany(a => a.Images)
            .HasForeignKey(ai => ai.ArtworkId); // FK for ArtworkImage references Artwork
        
        //Define the relationship between BioBlock and BioImage
        modelBuilder.Entity<BioImage>()
        .HasOne<BioBlock>()
        .WithMany(b => b.Images)
        .HasForeignKey(i => i.BioBlockId)
        .OnDelete(DeleteBehavior.Cascade);
    }
}