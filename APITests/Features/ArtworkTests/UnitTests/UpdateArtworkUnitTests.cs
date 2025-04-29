using System.Text;
using AutoMapper;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.Admin;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace IntegrationTests.Features.ArtworkTests.UnitTests;

public class UpdateArtworkUnitTests
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

    public UpdateArtworkUnitTests()
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
    public async Task UpdateArtworkAsync_ArtworkNotFoundInDatabase_ThrowsKeyNotFoundException()
    {
        // -- ARRANGE ----------
        var artworkId = 66;

        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest {Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync((Artwork)null);
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<KeyNotFoundException>(
            () => _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest)
        );
        
        // -- ASSERT ----------
        Assert.Equal($"Artwork with ID {artworkId} not found in database.", ex.Message);
    }
    
    
    // Empty Images List
    [Fact]
    public async Task UpdateArtworkAsync_EmptyImages_ThrowsException()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
            };
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock.Setup(repo => repo.UpdateArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => null);
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest)
        );
        
        // -- ASSERT ----------
        Assert.Equal("No image(s) found in the request.", ex.Message);
    }
    
    
    // Map from Request to Model
    [Fact]
    public async Task UpdateArtworkAsync_MapToModel_WhenRequestIsValid_ReturnsModel()
    {
        // -- ARRANGE ----------
        var artworkId = 66;

        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest {Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "New Title",
                Artist = "Someone new",
                Description = "New description",
                Materials = "New materials",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
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
        };
        
        _awsServiceMock.Setup(a => a.UploadThumbnailToS3(imageRequest.ImageFile)).ReturnsAsync("thumbnail-object-key");
        _awsServiceMock.Setup(a => a.UploadPreviewImageToS3(imageRequest.ImageFile)).ReturnsAsync("preview-object-key");
        _awsServiceMock.Setup(a => a.UploadImageToS3(imageRequest.ImageFile)).ReturnsAsync("full-view-object-key");
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync(existingArtwork);
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock.Setup(repo => repo.UpdateArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => a);
        
        // -- ACT ----------
        await _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest);
        
        
        // -- ASSERT ----------
        Assert.NotNull(artwork);
        Assert.IsType<Artwork>(artwork);
       
        // Artwork 
        Assert.NotNull(artwork.Id);
        Assert.Equal(artwork.Title, artworkRequest.Title);
        Assert.Equal(artwork.Description, artworkRequest.Description);
        Assert.Equal(artwork.Materials, artworkRequest.Materials);
        Assert.Equal(artwork.ForSale, artworkRequest.ForSale);
        Assert.Equal(artwork.Price, artworkRequest.Price);
        Assert.Equal(artwork.WidthDimension, artworkRequest.WidthDimension);
        Assert.Equal(artwork.HeightDimension, artworkRequest.HeightDimension);
        Assert.Equal(artwork.HomePageRotation, artworkRequest.HomePageRotation);
        // Image 
        Assert.Single(artwork.Images);
        Assert.NotNull(artwork.Images[0]);
        Assert.NotNull(artwork.Images[0].Id);
        Assert.NotNull(artwork.Images[0].ObjectKey);
        Assert.NotNull(artwork.Images[0].PreviewKey);
        Assert.NotNull(artwork.Images[0].ThumbnailKey);
        Assert.NotNull(artwork.Images[0].ArtworkId);
    }
    
    
    // Map from Model to Response is done in UploadArtworkUnitTests.cs using response = await _galleryService.GetArtworkByIdAsync(id);
    
    
    
    // Upload Only New Images
    [Fact]
    public async Task UpdateArtworkAsync_OnlyUploadsNewImagesToS3()
    {
        
    }
    
    
    // Delete Old Images
    [Fact]
    public async Task UpdateArtworkAsync_DeletesOldImagesFromS3()
    {
        // -- ARRANGE ----------
        var artworkId = 66;

        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest {Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "New Title",
                Artist = "Someone new",
                Description = "New description",
                Materials = "New materials",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };
        Image image1 = new Image
        {
            Id = 99, 
            ObjectKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
            ArtworkId = artworkId
        };
        Image image2 = new Image
        {
            Id = 99, 
            ObjectKey = "fullViewKey2", 
            PreviewKey = "previewKey2", 
            ThumbnailKey = "ThumbnailKey2", 
            ArtworkId = artworkId
        };
        Image image3 = new Image
        {
            Id = 99, 
            ObjectKey = "fullViewKey3", 
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
        
        var oldObjectKeys = new List<string>();
        var oldPreviewKeys = new List<string>();
        var oldThumbnailKeys = new List<string>();
        foreach (var img in existingArtwork.Images)
        {
            oldObjectKeys.Add(img.ObjectKey);
            oldThumbnailKeys.Add(img.ThumbnailKey);
            oldPreviewKeys.Add(img.PreviewKey);
        }
        
        _awsServiceMock.Setup(a => a.UploadThumbnailToS3(imageRequest.ImageFile)).ReturnsAsync("thumbnail-object-key");
        _awsServiceMock.Setup(a => a.UploadPreviewImageToS3(imageRequest.ImageFile)).ReturnsAsync("preview-object-key");
        _awsServiceMock.Setup(a => a.UploadImageToS3(imageRequest.ImageFile)).ReturnsAsync("full-view-object-key");
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync(existingArtwork);
        
        
        // -- ACT ----------
        await _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest);
        
        // -- ACT ----------
        foreach (var key in oldObjectKeys)
        {
            _awsServiceMock.Verify(aws => aws.DeleteImageFromS3(key), Times.Once);
        }
        foreach (var key in oldThumbnailKeys)
        {
            _awsServiceMock.Verify(aws => aws.DeleteImageFromS3(key), Times.Once);
        }
        foreach (var key in oldPreviewKeys)
        {
            _awsServiceMock.Verify(aws => aws.DeleteImageFromS3(key), Times.Once);
        }
        
    }
    
    
    // Repository Update Verification
    [Fact]
    public async Task UpdateArtworkAsync_WhenValidRequestAndIdFoundInDatabase_UpdatesArtwork()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest {Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "New Title",
                Artist = "Someone new",
                Description = "New description",
                Materials = "New materials",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };
        
        Image image = new Image
        {
            Id = 99, 
            ObjectKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
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
                image
            } 
        };
        
        _awsServiceMock
            .Setup(a => a.UploadImageToS3(It.IsAny<IFormFile>()))
            .ReturnsAsync("newFullViewKey");
        _awsServiceMock
            .Setup(a => a.UploadPreviewImageToS3(It.IsAny<IFormFile>()))
            .ReturnsAsync("newPreviewKey");
        _awsServiceMock
            .Setup(a => a.UploadThumbnailToS3(It.IsAny<IFormFile>()))
            .ReturnsAsync("newThumbKey");
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync(existingArtwork);
        
        
        // -- ACT ----------
        await _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest);
        
        // -- ASSERT ----------
        _adminGalleryRepositoryMock.Verify(r => 
            r.UpdateArtworkAsync(It.Is<Artwork>(art => art.Id == artworkId)), Times.Once);
        
    }
    
    
    // Repository Failure
    [Fact]
    public async Task UpdateArtworkAsync_WhenRepositoryFails_ThrowsInvalidOperationException()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest {Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "New Title",
                Artist = "Someone new",
                Description = "New description",
                Materials = "New materials",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };
        Image image = new Image
        {
            Id = 99, 
            ObjectKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
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
                image
            } 
        };
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ThrowsAsync(new InvalidOperationException("DB is down"));
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest)
        );
        
        // -- ASSERT ----------
        Assert.Equal("DB is down", ex.Message);
    }
    
    
    // Aws Failure
    [Fact]
    public async Task UpdateArtworkAsync_WhenAwsFails_ThrowsIOException()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        UpdateImageRequest imageRequest = new UpdateImageRequest {Id = 99, ImageFile = imageFile, ArtworkId = artworkId };
        
        // Mocking Artwork Requests
        UpdateArtworkRequest artworkRequest = 
            new ()
            {
                Title = "New Title",
                Artist = "Someone new",
                Description = "New description",
                Materials = "New materials",
                ForSale = true,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new ()
                {
                    imageRequest
                }
            };
        Image image = new Image
        {
            Id = 99, 
            ObjectKey = "fullViewKey1", 
            PreviewKey = "previewKey1", 
            ThumbnailKey = "ThumbnailKey1", 
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
                image
            } 
        };
        
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync(existingArtwork);
        
        _awsServiceMock
            .Setup(a => a.UploadImageToS3(It.IsAny<IFormFile>()))
            .ThrowsAsync(new IOException("S3 is down"));
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<IOException>(
            () => _adminGalleryService.UpdateArtworkAsync(artworkId, artworkRequest)
        );
        
        // -- ASSERT ----------
        Assert.Contains("S3 is down", ex.Message);
    }
    
    
}