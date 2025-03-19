
using AutoMapper;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using JohnsenArtAPI.Services.Interfaces;

namespace JohnsenArtAPI.Services;

public class AdminGalleryService : IAdminGalleryService
{
    private readonly IAdminGalleryRepository _repository;
    private readonly IGalleryRepository _repoGet;
    private readonly IAwsService _aws;
    private readonly IMapper _mapper;
    private readonly ILogger<AdminGalleryService> _logger;

    public AdminGalleryService(
        IAdminGalleryRepository repository,
        IGalleryRepository repoGet,
        IAwsService aws,
        IMapper mapper,
        ILogger<AdminGalleryService> logger)
    {
        _repository = repository;
        _repoGet = repoGet;
        _aws = aws;
        _mapper = mapper;
        _logger = logger;
    }
    
    // UPLOAD Artwork
    public async Task<ArtworkResponse> UploadArtworkAsync(ArtworkRequest request)
    {
        _logger.LogInformation($"-------------------- \n Service: UploadArtwork:");
        _logger.LogDebug($"Number of images in the request: {request.Images?.Count}");
        
        
        // Making sure bucket exists (It declared in appsettings.json)
        await _aws.CheckIfS3BucketExists();

        // Map DTO to Entity
        var artwork = _mapper.Map<Artwork>(request);
        artwork.Images.Clear(); // Remove Automapper placeholders

        // Uploading image(s) to S3 bucket
        foreach (var image in request.Images)
        {
            _logger.LogDebug("Entering image upload loop.");
            _logger.LogDebug($"Processing image: {image.ImageFile?.FileName ?? "No file"}");

            if (image.ImageFile != null)
            {
                var objectKey = await _aws.UploadImageToS3(image.ImageFile);

                // Adding ObjectKey property value to Artwork entity
                artwork.Images.Add(new ArtworkImage { ObjectKey = objectKey, IsWallPreview = image.IsWallPreview });
            }
            else
            {
                _logger.LogWarning($"No image found in image upload loop.");
                throw new Exception("No image found in image upload loop.");
            }
        }
        
        // Saving to database through repository
        var savedArtwork = await _repository.AddArtworkAsync(artwork);

        // Return DTO
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }
    
    // UPDATE Artwork
    public async Task<ArtworkResponse?> UpdateArtworkAsync(int artId, UpdateArtworkRequest request)
    {
        _logger.LogInformation($"------------------- \n Service: UpdateArtwork ");

        var existingArtwork = await _repoGet.GetArtworkByIdAsync(artId);
        if (existingArtwork == null)
        {
            _logger.LogWarning($"Artwork with ID {artId} not found.");
            return null;
        }

        _logger.LogDebug($"Number of images in the request: {request.Images?.Count}");

        // Only update properties that are explicitly provided in the request
        if (request.Title is not null) existingArtwork.Title = request.Title;
        if (request.Description is not null) existingArtwork.Description = request.Description;
        if (request.Artist is not null) existingArtwork.Artist = request.Artist;
        if (request.Price.HasValue) existingArtwork.Price = request.Price;
        if (request.HeightDimension is not null) existingArtwork.HeightDimension = request.HeightDimension;
        if (request.WidthDimension is not null) existingArtwork.WidthDimension = request.WidthDimension;
        if (request.ForSale is not null) existingArtwork.Price = request.Price;
        
        // Handle images (managing updates and deletions separately)
        UpdateArtworkImages(existingArtwork, request.Images);

        var savedArtwork = await _repository.UpdateArtworkAsync(existingArtwork);
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }

    private async void UpdateArtworkImages(Artwork existingArtwork, List<UpdateImageRequest> images)
    {
        
        foreach (var image in images)
        {
            if (image.ImageFile == null) continue;

            if (image.Id is null)
            {
                _logger.LogWarning($"No image ID provided, skipping image update.");
                continue;
            }
            
            // Replace existing image object
            var newImage = new ArtworkImage
            {
                ObjectKey = _aws.UploadImageToS3(image.ImageFile).Result, // Upload image
                IsWallPreview = image.IsWallPreview
            };
            
            // Adding images to existing artwork
            existingArtwork.Images.Add(newImage);
            
            // Getting object key from database
            var oldObjectKey = await _repository.GetObjectKeyByImageIdAsync(image.Id);
            
            // Deleting old image for S3
            await _aws.DeleteImageFromS3(oldObjectKey!);
            
            
            
        }
    }
    /*
    public async Task<ArtworkResponse?> UpdateArtworkAsync(int artId, ArtworkRequest request)
    {
        _logger.LogInformation($"------------------- \n Service: UpdateArtwork ");
        
        // Map DTO to Entity
        var artwork = _mapper.Map<Artwork>(request);
        artwork.Images.Clear(); // Remove Automapper placeholders
        artwork.Id = artId;
        
        // Uploading image(s) to S3 bucket
        foreach (var image in request.Images)
        {
            _logger.LogDebug("Entering image upload loop.");
            _logger.LogDebug($"Processing image: {image.ImageFile?.FileName ?? "No file"}");
            
            // If image is not being updated -> exit loop
            if (image.ImageFile == null) continue;
            
            // If no image ID is provided -> exit loop
            if (image.Id is null)
            {
                _logger.LogWarning($"No image ID provided from client, no image will be updated.");
                continue;
            }
            
            // Getting object key from database
            var oldObjectKey = await _repository.GetObjectKeyByImageIdAsync(image.Id);
            
            // Deleting old image for S3
            await _aws.DeleteImageFromS3(oldObjectKey!);
            
            // Uploading new image to S3
            var newObjectKey = await _aws.UploadImageToS3(image.ImageFile);

            // Adding new ObjectKey property value to Artwork entity
            artwork.Images.Add(new ArtworkImage { ObjectKey = newObjectKey, IsWallPreview = image.IsWallPreview });
        }
        
        var savedArtwork = await _repository.UpdateArtworkAsync(artwork);
        return _mapper.Map<ArtworkResponse>(savedArtwork);
        
    }*/
    
    // DELETE Artwork
    public async Task<ArtworkResponse?> DeleteArtworkAsync(int artId)
    {
        _logger.LogInformation($"------------------- \n Service: DeleteArtwork : with ID {artId}");
        
        var deletedArtwork = await _repository.DeleteArtworkAsync(artId);

        if (deletedArtwork is not null)
        {
            foreach (var image in deletedArtwork.Images)
            {
                if (image is null) continue;
                await _aws.DeleteImageFromS3(image.ObjectKey);
                _logger.LogInformation($"Deleted image: {image.ObjectKey}");
            }
        }
        
        return _mapper.Map<ArtworkResponse>(deletedArtwork);
    }
}