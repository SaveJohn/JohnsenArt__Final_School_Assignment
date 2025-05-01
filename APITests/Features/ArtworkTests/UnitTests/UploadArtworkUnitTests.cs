using System.Text;
using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess;
using JohnsenArtAPI.Features.Gallery.Common;
using JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NuGet.Frameworks;
using Xunit;

namespace IntegrationTests.Features.ArtworkTests.UnitTests;

public class UploadArtworkUnitTests
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

    public UploadArtworkUnitTests()
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
    
    // Empty Images
    [Fact]
    public async Task UploadArtworkAsync_EmptyImages_ThrowsException()
    {
        // -- ARRANGE ----------
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _adminGalleryService.UploadArtworkAsync(artworRequest)
        );
        
        // -- ASSERT ----------
        Assert.Equal("No image(s) found in the request.", ex.Message);
        
    }
    
    // File-upload Calls
    [Fact]
    public async Task UploadArtworkAsync_CallsBucketCheckExactlyOnce()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        
        // -- ACT ----------
        await _adminGalleryService.UploadArtworkAsync(artworRequest);

        // -- ASSERT ----------
        _awsServiceMock.Verify(a => a.CheckIfS3BucketExists(), Times.Once);
    }
    
    // Image upload
    [Fact]
    public async Task UploadArtworkAsync_UploadsEachImageOnce()
    {
        // -- ARRANGE ----------
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile1 = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile1.png");
        IFormFile imageFile2 = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile2.png");
        IFormFile imageFile3 = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile3.png");

        ImageRequest imageRequest1 = new ImageRequest { ImageFile = imageFile1 };
        ImageRequest imageRequest2 = new ImageRequest { ImageFile = imageFile2 };
        ImageRequest imageRequest3 = new ImageRequest { ImageFile = imageFile3 };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest1,
                    imageRequest2,
                    imageRequest3
                }
            };
        
        // -- ACT ----------
        await _adminGalleryService.UploadArtworkAsync(artworRequest);
        
        // -- ASSERT ---------- 
        
        _awsServiceMock.Verify(a => a.UploadThumbnailToS3(imageRequest1.ImageFile), Times.Once);
        _awsServiceMock.Verify(a => a.UploadPreviewImageToS3(imageRequest1.ImageFile), Times.Once);
        _awsServiceMock.Verify(a => a.UploadImageToS3(imageRequest1.ImageFile), Times.Once);
        
        _awsServiceMock.Verify(a => a.UploadThumbnailToS3(imageRequest2.ImageFile), Times.Once);
        _awsServiceMock.Verify(a => a.UploadPreviewImageToS3(imageRequest2.ImageFile), Times.Once);
        _awsServiceMock.Verify(a => a.UploadImageToS3(imageRequest2.ImageFile), Times.Once);
        
        _awsServiceMock.Verify(a => a.UploadThumbnailToS3(imageRequest3.ImageFile), Times.Once);
        _awsServiceMock.Verify(a => a.UploadPreviewImageToS3(imageRequest3.ImageFile), Times.Once);
        _awsServiceMock.Verify(a => a.UploadImageToS3(imageRequest3.ImageFile), Times.Once);
    }
    
    // Image Entity Population
    [Fact]
    public async Task UploadArtworkAsync_PopulatesImageObjectKeysCorrectly()
    {
        // -- ARRANGE ----------
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile1 = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile1.png");
        ImageRequest imageRequest1 = new ImageRequest { ImageFile = imageFile1 };
       
        IFormFile imageFile2 = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile1.png");
        ImageRequest imageRequest2 = new ImageRequest { ImageFile = imageFile2 };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest1,
                    imageRequest2
                }
            };

        _awsServiceMock.Setup(a => a.UploadThumbnailToS3(imageRequest1.ImageFile)).ReturnsAsync("thumbnail-object-key-1");
        _awsServiceMock.Setup(a => a.UploadPreviewImageToS3(imageRequest1.ImageFile)).ReturnsAsync("preview-object-key-1");
        _awsServiceMock.Setup(a => a.UploadImageToS3(imageRequest1.ImageFile)).ReturnsAsync("full-view-object-key-1");
        
        _awsServiceMock.Setup(a => a.UploadThumbnailToS3(imageRequest2.ImageFile)).ReturnsAsync("thumbnail-object-key-2");
        _awsServiceMock.Setup(a => a.UploadPreviewImageToS3(imageRequest2.ImageFile)).ReturnsAsync("preview-object-key-2");
        _awsServiceMock.Setup(a => a.UploadImageToS3(imageRequest2.ImageFile)).ReturnsAsync("full-view-object-key-2");
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock
            .Setup(r => r.UploadArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => a);
        
        // -- ACT ----------
        await _adminGalleryService.UploadArtworkAsync(artworRequest);
        
        // -- ASSERT ----------
        Assert.NotNull(artwork);
        Assert.Equal(2, artwork.Images.Count);
        
        // Image 1
        var img1 = artwork.Images[0];
        Assert.NotNull(img1);
        Assert.Equal("thumbnail-object-key-1", img1.ThumbnailKey);
        Assert.Equal("preview-object-key-1", img1.PreviewKey);
        Assert.Equal("full-view-object-key-1", img1.FullViewKey);
        
        var img2 = artwork.Images[1];
        Assert.NotNull(img2);
        Assert.Equal("thumbnail-object-key-2", img2.ThumbnailKey);
        Assert.Equal("preview-object-key-2", img2.PreviewKey);
        Assert.Equal("full-view-object-key-2", img2.FullViewKey);
        
    }
    
    // Price Clearing
    [Fact]
    public async Task UploadArtworkAsync_ClearsPriceWhenNotForSale()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
            new ()
            {
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = false,
                Price = 7000,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock
            .Setup(r => r.UploadArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => a);
        
        // -- ACT ----------
        await _adminGalleryService.UploadArtworkAsync(artworRequest);
        
        // -- ASSERT ----------
        Assert.NotNull(artwork);
        Assert.Null(artwork.Price);
    }
    
    // For Sale Price
    [Fact]
    public async Task UploadArtworkAsync_WhenPriceIsNullAndForSaleIsTrue_ThrowsException()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
            new ()
            {
                Title = "Lonely Mountain",
                Artist = "Bilbo Baggins",
                Description = "Far away - in a Lonely Mountain - rests Smaug the greedy dragon. There he is guarding his loot, and a stolen home.",
                Materials = "Ink on canvas",
                ForSale = true,
                Price = null,
                WidthDimension = 50f,
                HeightDimension = 90f,
                HomePageRotation = false,
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock
            .Setup(r => r.UploadArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => a);
        
        // -- ACT & ASSERT ----------
        var ex = await Assert.ThrowsAsync<Exception>(
            () => _adminGalleryService.UploadArtworkAsync(artworRequest)
        );
        Assert.Equal("Price can't be null when request for sale is true.", ex.Message);
    }
    
    // Repository Save
    [Fact]
    public async Task UploadArtworkAsync_CallsRepositoryOnce()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock
            .Setup(r => r.UploadArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => a);
        
        // -- ACT ----------
        await _adminGalleryService.UploadArtworkAsync(artworRequest);
        
        // -- ASSERT ----------
        _adminGalleryRepositoryMock.Verify(r => r.UploadArtworkAsync(It.IsAny<Artwork>()), Times.Once);
            
    }
    
    // Map from Request to Model
    [Fact]
    public async Task UploadArtworkAsync_MapToModel_WhenRequestIsValid_ReturnsModel()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        _awsServiceMock.Setup(a => a.UploadThumbnailToS3(imageRequest.ImageFile)).ReturnsAsync("thumbnail-object-key");
        _awsServiceMock.Setup(a => a.UploadPreviewImageToS3(imageRequest.ImageFile)).ReturnsAsync("preview-object-key");
        _awsServiceMock.Setup(a => a.UploadImageToS3(imageRequest.ImageFile)).ReturnsAsync("full-view-object-key");
        
        Artwork artwork = null;
        _adminGalleryRepositoryMock.Setup(repo => repo.UploadArtworkAsync(It.IsAny<Artwork>()))
            .Callback<Artwork>(a => artwork = a)
            .ReturnsAsync((Artwork a) => a);
        
        // -- ACT ----------
        await _adminGalleryService.UploadArtworkAsync(artworRequest);
        
        
        // -- ASSERT ----------
        Assert.NotNull(artwork);
        Assert.IsType<Artwork>(artwork);
       
        // Artwork 
        Assert.NotNull(artwork.Id);
        Assert.Equal(artwork.Title, artworRequest.Title);
        Assert.Equal(artwork.Description, artworRequest.Description);
        Assert.Equal(artwork.Materials, artworRequest.Materials);
        Assert.Equal(artwork.ForSale, artworRequest.ForSale);
        Assert.Equal(artwork.Price, artworRequest.Price);
        Assert.Equal(artwork.WidthDimension, artworRequest.WidthDimension);
        Assert.Equal(artwork.HeightDimension, artworRequest.HeightDimension);
        Assert.Equal(artwork.HomePageRotation, artworRequest.HomePageRotation);
        // Image 
        Assert.Single(artwork.Images);
        Assert.NotNull(artwork.Images[0]);
        Assert.NotNull(artwork.Images[0].Id);
        Assert.NotNull(artwork.Images[0].FullViewKey);
        Assert.NotNull(artwork.Images[0].PreviewKey);
        Assert.NotNull(artwork.Images[0].ThumbnailKey);
        Assert.NotNull(artwork.Images[0].ArtworkId);
    }
    
    
    // Map from Model to Response
    [Fact]
    public async Task UploadArtworkAsync_MapToResponse_WhenModelIsValid_ReturnsResponse()
    {
        // -- ARRANGE ----------
        var artworkId = 66;
        // Mocking Image 
        Image? image = new()
        {
            Id = artworkId,
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
        _galleryRepositoryMock
            .Setup(r => r.GetArtworkByIdAsync(artworkId))
            .ReturnsAsync(artwork);
        
        _awsServiceMock.Setup(a => a.GeneratePresignedUrl(image.ThumbnailKey)).Returns("thumbnail-url");
        _awsServiceMock.Setup(a => a.GeneratePresignedUrl(image.PreviewKey)).Returns("preview-url");
        _awsServiceMock.Setup(a => a.GeneratePresignedUrl(image.FullViewKey)).Returns("full-view-url");
        
        // -- ACT ----------
        
        ArtworkResponse response = await _galleryService.GetArtworkByIdAsync(66);
        
        
        
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
        Assert.NotNull(response.Images[0].FullViewUrl);
        Assert.NotNull(response.Images[0].PreviewUrl);
        Assert.NotNull(response.Images[0].ThumbnailUrl);
    }
    
    // Aws Exceptions
    [Fact]
    public async Task UploadArtworkAsync_WhenAwsFails_ThrowsIOException()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        
        _awsServiceMock
            .Setup(a => a.UploadImageToS3(It.IsAny<IFormFile>()))
            .ThrowsAsync(new IOException("S3 is down"));
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<IOException>(
            () => _adminGalleryService.UploadArtworkAsync(artworRequest)
        );
        
        // -- ASSERT ----------
        _awsServiceMock.Verify(a => a.UploadImageToS3(It.IsAny<IFormFile>()), Times.Once);
        Assert.Contains("S3 is down", ex.Message);
    }
    
    // Repository Exceptions
    [Fact]
    public async Task UploadArtworkAsync_WhenRepositoryFails_ThrowsInvalidOperationException()
    {
        // -- ARRANGE ----------
        // Mocking Image Requests
        var bytes = Encoding.UTF8.GetBytes("This is a dummy image file");
        IFormFile imageFile = new FormFile(new MemoryStream(bytes), 0, bytes.Length, "Data", "imageFile.png");

        ImageRequest imageRequest = new ImageRequest { ImageFile = imageFile };
        
        // Mocking Artwork Requests
        ArtworkRequest artworRequest = 
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
                Images = new List<ImageRequest>()
                {
                    imageRequest
                }
            };
        
        _adminGalleryRepositoryMock.Setup(r => r.UploadArtworkAsync(It.IsAny<Artwork>()))
            .ThrowsAsync(new InvalidOperationException("DB is down"));
        
        
        // -- ACT ----------
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(
            () => _adminGalleryService.UploadArtworkAsync(artworRequest)
        );
        
        // -- ASSERT ----------
        _adminGalleryRepositoryMock.Verify(r => r.UploadArtworkAsync(It.IsAny<Artwork>()), Times.Once);
        Assert.Equal("DB is down", ex.Message);
    }
    
}