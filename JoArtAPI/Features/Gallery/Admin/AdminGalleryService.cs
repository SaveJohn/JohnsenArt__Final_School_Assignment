using AutoMapper;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Services.Interfaces;

namespace JohnsenArtAPI.Features.Gallery.Admin;

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
        if (!request.ForSale) artwork.Price = null;
        
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

        
        _mapper.Map(request, existingArtwork);
        existingArtwork.Id = artId;
        if (!request.ForSale) existingArtwork.Price = null;
        existingArtwork.Images.Clear();
        
        // Handle images (managing updates and deletions separately)
        await UpdateArtworkImages(existingArtwork, request.Images);

        var savedArtwork = await _repository.UpdateArtworkAsync(existingArtwork);
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }

    private async Task UpdateArtworkImages(Artwork existingArtwork, List<UpdateImageRequest> images)
    {
        _logger.LogInformation($"------------------- \n Service: UpdateArtworkImages ");
        _logger.LogDebug($"Number of images in the request: {images.Count}");
        
        // Storing old Object Keys to delete from S3 if update in database is successful 
        var oldObjectKeys = new List<string>();
        foreach (var image in existingArtwork.Images)
        {
            oldObjectKeys.Add(image.ObjectKey);
        }

        try
        {
            foreach (var image in images)
            {
                if (image.ImageFile == null) continue;
                
                // Replace existing image object
                var newImage = new ArtworkImage
                {
                    Id = image.Id,
                    ArtworkId = existingArtwork.Id,
                    ObjectKey = _aws.UploadImageToS3(image.ImageFile).Result, // Upload image
                    IsWallPreview = image.IsWallPreview
                };

                // Adding images to existing artwork
                existingArtwork.Images.Add(newImage);

            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occured while updating images: {ex.Message}");
            throw;
        }
        
        // Deleting old images from S3
        foreach (var objectKey in oldObjectKeys)
        {
            await _aws.DeleteImageFromS3(objectKey);
        }
        
    }
    
    
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