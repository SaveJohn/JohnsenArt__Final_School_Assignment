using System.Text;
using AutoMapper;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess;
using JohnsenArtAPI.Features.Gallery.Common;
using JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IntegrationTests.Features.ArtworkTests.UnitTests;

public class DeleteArtworkUnitTests
{
    // Services
    private readonly AdminGalleryService _adminGalleryService;
    private readonly GalleryService _galleryService;
    private readonly IMapper _mapper;
    
    // Mocks
    private readonly Mock<ILogger<AdminGalleryService>> _adminLoggerMock = new();
    private readonly Mock<ILogger<GalleryService>> _loggerMock = new();
    private readonly Mock<IAwsService> _awsServiceMock = new();
    private readonly Mock<IAdminGalleryRepository> _adminGalleryRepositoryMock = new();
    private readonly Mock<IGalleryRepository> _galleryRepositoryMock = new();

    public DeleteArtworkUnitTests()
    {
        // Configuring AutoMapper 
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new ArtworkMapper());
        });
        _mapper = config.CreateMapper();
        
        // Make sure CheckIfS3BucketExists() check out for all unit tests
        _awsServiceMock.Setup(a => a.CheckIfS3BucketExists())
            .ReturnsAsync(true);
        
        // Admin Gallery Service constructor
        _adminGalleryService = new AdminGalleryService(
            _adminGalleryRepositoryMock.Object,
            _galleryRepositoryMock.Object,
            _awsServiceMock.Object,
            _mapper,
            _adminLoggerMock.Object
            
        );
        
        // Gallery Service constructor
        _galleryService = new GalleryService(
            _galleryRepositoryMock.Object,
            _awsServiceMock.Object,
            _mapper,
            _loggerMock.Object
            
        );
    }
    
    
    // No Existing Artwork
    [Fact]
    public async Task DeleteArtworkAsync_ArtworkNotFoundInDatabase_ThrowsKeyNotFoundException()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync((Artwork)null);
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _adminGalleryService.DeleteArtworkAsync(artworkId)
        );
        
        // -- ASSERT ----------
        Assert.Equal($"Artwork with ID {artworkId} not found in database.", ex.Message);
    }
    
    // Delete Old Images From S3
    [Fact]
    public async Task DeleteArtworkAsync_DeletesOldImagesFromS3()
    {
        // -- ARRANGE ----------
        var artworkId = 66;

        
        Image image1 = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
            ArtworkId = artworkId
        };
        Image image2 = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey2", 
            PreviewKey = "previewKey2", 
            ThumbnailKey = "ThumbnailKey2", 
            ArtworkId = artworkId
        };
        Image image3 = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey3", 
            PreviewKey = "previewKey3", 
            ThumbnailKey = "ThumbnailKey3", 
            ArtworkId = artworkId
        };
        
        var existingArtwork = new Artwork {
            Id               = artworkId,
            Title            = "Old title",
            Artist           = "Someone Else",
            Description      = "Old description",
            Materials        = "Old materials",
            ForSale          = false,
            Price            = null,
            WidthDimension = 90f,
            HeightDimension = 50f,
            HomePageRotation = true,
            Images = new ()
            {
                image1,
                image2,
                image3
            } 
        };
        
        _adminGalleryRepositoryMock
            .Setup(r => r.DeleteArtworkAsync(artworkId))
            .ReturnsAsync(existingArtwork);
        
        // -- ACT ----------
        await _adminGalleryService.DeleteArtworkAsync(artworkId);
        
        // -- ASSERT ----------
        foreach (var img in existingArtwork.Images)
        {
            _awsServiceMock.Verify(aws => aws.DeleteImageFromS3(img.FullViewKey), Times.Once);
            _awsServiceMock.Verify(aws => aws.DeleteImageFromS3(img.PreviewKey), Times.Once);
            _awsServiceMock.Verify(aws => aws.DeleteImageFromS3(img.ThumbnailKey), Times.Once);
        }
        
    }
    
    // Repository call
    [Fact]
    public async Task DeleteArtworkAsync_CallsRepositoryOnce()
    {
        // -- ARRANGE ----------
        var artworkId = 66;

        
        Image image1 = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
            ArtworkId = artworkId
        };
        Image image2 = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey2", 
            PreviewKey = "previewKey2", 
            ThumbnailKey = "ThumbnailKey2", 
            ArtworkId = artworkId
        };
        Image image3 = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey3", 
            PreviewKey = "previewKey3", 
            ThumbnailKey = "ThumbnailKey3", 
            ArtworkId = artworkId
        };
        
        var existingArtwork = new Artwork {
            Id               = artworkId,
            Title            = "Old title",
            Artist           = "Someone Else",
            Description      = "Old description",
            Materials        = "Old materials",
            ForSale          = false,
            Price            = null,
            WidthDimension = 90f,
            HeightDimension = 50f,
            HomePageRotation = true,
            Images = new ()
            {
                image1,
                image2,
                image3
            } 
        };
        
        _adminGalleryRepositoryMock
            .Setup(r => r.DeleteArtworkAsync(artworkId))
            .ReturnsAsync(existingArtwork);
        
        // -- ACT ----------
        await _adminGalleryService.DeleteArtworkAsync(artworkId);
        
        // -- ASSERT ----------
        _adminGalleryRepositoryMock.Verify(r => r.DeleteArtworkAsync(artworkId), Times.Once);
    }
    
    
    // mapping
    [Fact]
    public async Task DeleteArtworkAsync_WithExistingId_ShouldReturnCorrectArtworkResponse()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        // Mocking Image 
        Image? image = new()
        {
            Id = 99,
            FullViewKey = "thumbnail-object-key",
            PreviewKey = "preview-object-key",
            ThumbnailKey = "thumbnail-object-key",
            ArtworkId = 66
        };
        
        // Mocking Artwork 
        Artwork artwork = 
            new ()
            {
                Id = 66,
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new List<Image?>()
                {
                    image
                }
            };
        
        _adminGalleryRepositoryMock.Setup(r => r.DeleteArtworkAsync(artworkId))
            .ReturnsAsync(artwork);
        
        
        // -- ACT ----------
        
        ArtworkResponse response = await _adminGalleryService.DeleteArtworkAsync(artworkId);
        
        
        // -- ASSERT ----------
        Assert.NotNull(response);
        Assert.IsType<ArtworkResponse>(response);
       
        // Artwork 
        Assert.Equal(artwork.Title, response.Title);
        Assert.Equal(artwork.Description, response.Description);
        Assert.Equal(artwork.Materials, response.Materials);
        Assert.Equal(artwork.ForSale, response.ForSale);
        Assert.Equal(artwork.Price, response.Price);
        Assert.Equal(artwork.WidthDimension, response.WidthDimension);
        Assert.Equal(artwork.HeightDimension, response.HeightDimension);
        Assert.Equal(artwork.HomePageRotation, response.HomePageRotation);
        // Image 
        Assert.Single(response.Images);
        Assert.NotNull(response.Images[0]);
        Assert.NotNull(response.Images[0].FullViewKey);
        Assert.NotNull(response.Images[0].PreviewKey);
        Assert.NotNull(response.Images[0].ThumbnailKey);
    }
    
    
    // Aws failure
    [Fact]
    public async Task DeleteArtworkAsync_WhenAwsFails_ThrowsIOException()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        Image image = new Image
        {
            Id = 99, 
            FullViewKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
            ArtworkId = artworkId
        };
        
        var artwork = new Artwork {
            Id               = artworkId,
            Title            = "Old title",
            Artist           = "Someone Else",
            Description      = "Old description",
            Materials        = "Old materials",
            ForSale          = false,
            Price            = null,
            WidthDimension = 90f,
            HeightDimension = 50f,
            HomePageRotation = true,
            Images = new ()
            {
                image
            } 
        };
        
        _adminGalleryRepositoryMock.Setup(r => r.DeleteArtworkAsync(artworkId))
            .ReturnsAsync(artwork);
        
        _awsServiceMock
            .Setup(a => a.DeleteImageFromS3(It.IsAny<string>()))
            .ThrowsAsync(new IOException("S3 is down"));
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<IOException>(
            () => _adminGalleryService.DeleteArtworkAsync(artworkId)
        );
        
        // -- ASSERT ----------
        _awsServiceMock.Verify(a => a.DeleteImageFromS3("fullViewKey1"), Times.Once);
        Assert.Contains("S3 is down", ex.Message);
        
    }
}