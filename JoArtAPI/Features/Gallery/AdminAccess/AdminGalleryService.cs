using AutoMapper;
using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.AdminAccess.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;

namespace JohnsenArtAPI.Features.Gallery.AdminAccess;

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
        
        // Making sure artwork contains images
        if (request.Images == null || request.Images.Count == 0)
        {
            _logger.LogError($"No image(s) found in the request.");
            throw new Exception("No image(s) found in the request.");
        }

        if (request.ForSale && request.Price is null)
        {
            _logger.LogError("Price can't be null when request for sale is true.");
            throw new Exception("Price can't be null when request for sale is true.");
        }
        
        // Map request to model
        var artwork = _mapper.Map<Artwork>(request);
        if (!request.ForSale) artwork.Price = null;
        artwork.Images.Clear(); // Remove Automapper placeholders
        
        // Making sure bucket exists (It is declared in appsettings.json)
        await _aws.CheckIfS3BucketExists();
        
        // Uploading image(s) to S3 bucket
        foreach (var image in request.Images)
        {
            _logger.LogDebug("Entering image upload loop.");
            _logger.LogDebug($"Processing image: {image.ImageFile?.FileName ?? "No file"}");

            if (image.ImageFile != null)
            {
                
                var fullKey = await _aws.UploadImageToS3(image.ImageFile);
                var previewKey = await _aws.UploadPreviewImageToS3(image.ImageFile);
                var thumbKey = await _aws.UploadThumbnailToS3(image.ImageFile);

                artwork.Images.Add(new Image
                {
                    
                    FullViewKey = fullKey,
                    ThumbnailKey = thumbKey,
                    PreviewKey = previewKey
                });
            }
            else
            {
                _logger.LogWarning($"No image file(s) found in image upload loop.");
                throw new Exception("No image files(s) found in image upload loop.");
            }
        }


        // Saving to database through repository
        var savedArtwork = await _repository.UploadArtworkAsync(artwork);

        // Return DTO
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }

    // UPDATE Artwork
    public async Task<ArtworkResponse?> UpdateArtworkAsync(int artId, UpdateArtworkRequest request)
    {
        _logger.LogInformation($"------------------- \n Service: UpdateArtwork ");
        
        if (request.Images == null || request.Images.Count == 0)
        {
            _logger.LogError($"No image(s) found in the request.");
            throw new Exception("No image(s) found in the request.");
        }
        
        if (request.ForSale && request.Price is null)
        {
            _logger.LogError("Price can't be null when request for sale is true.");
            throw new Exception("Price can't be null when request for sale is true.");
        }
        
        var existingArtwork = await _repoGet.GetArtworkByIdAsync(artId);
        if (existingArtwork is null)
        {
            _logger.LogError($"Artwork with ID {artId} not found in database.");
            throw new KeyNotFoundException($"Artwork with ID {artId} not found in database.");
        }
        List<Image> existingImages = existingArtwork.Images.ToList();


        _mapper.Map(request, existingArtwork);
        existingArtwork.Id = artId;
        if (!request.ForSale) existingArtwork.Price = null;

        // Handle images (managing updates and deletions separately)
        await UpdateArtworkImages(existingArtwork, request.Images, existingImages);

        var savedArtwork = await _repository.UpdateArtworkAsync(existingArtwork);
        
        return _mapper.Map<ArtworkResponse>(savedArtwork);
    }

    private async Task UpdateArtworkImages(Artwork existingArtwork, List<UpdateImageRequest> newImages, List<Image> oldImages)
    {
        _logger.LogInformation($"------------------- \n Service: UpdateArtworkImages ");
        _logger.LogDebug($"Number of images in the request: {newImages.Count}");
        
        
        // Storing old Object Keys to delete from S3 if update in database is successful 
        var oldFullViewKeys = new List<string>();
        var oldPreviewKeys = new List<string>();
        var oldThumbnailKeys = new List<string>();
        foreach (var image in oldImages)
        {
            oldFullViewKeys.Add(image.FullViewKey);
            oldThumbnailKeys.Add(image.ThumbnailKey);
            oldPreviewKeys.Add(image.PreviewKey);
        }
        
        existingArtwork.Images.Clear();
        
        try
        {
            // Making sure bucket exists (It is declared in appsettings.json)
            await _aws.CheckIfS3BucketExists();
            
            foreach (var image in newImages)
            {
                if (image.ImageFile == null) continue;

                // Replace existing image object
                var newImage = new Image
                {
                    Id = image.Id,
                    ArtworkId = existingArtwork.Id,
                    FullViewKey = await _aws.UploadImageToS3(image.ImageFile), // Upload image
                    PreviewKey = await _aws.UploadPreviewImageToS3(image.ImageFile), //Upload preview
                    ThumbnailKey = await _aws.UploadThumbnailToS3(image.ImageFile), //Upload thumbnail
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

        try
        {
            // Deleting old images from S3
            foreach (var objectKey in oldFullViewKeys) await _aws.DeleteImageFromS3(objectKey);
        
            // Deleting old previews from S3
            foreach (var objectKey in oldPreviewKeys) await _aws.DeleteImageFromS3(objectKey);
        
            // Deleting old thumbnails from S3
            foreach (var objectKey in oldThumbnailKeys) await _aws.DeleteImageFromS3(objectKey);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error Deleting old images from S3: {ex.Message}");
            throw;
        }
        
        
    }


    // DELETE Artwork
    public async Task<ArtworkResponse?> DeleteArtworkAsync(int artId)
    {
        _logger.LogInformation($"------------------- \n Service: DeleteArtwork : with ID {artId}");

        var deletedArtwork = await _repository.DeleteArtworkAsync(artId);
        if (deletedArtwork is null)
        {
            _logger.LogError($"Artwork with ID {artId} not found in database.");
            throw new KeyNotFoundException($"Artwork with ID {artId} not found in database.");
        }
        
        foreach (var image in deletedArtwork.Images)
        {
            if (image is null) continue;
            await _aws.DeleteImageFromS3(image.FullViewKey);
            await _aws.DeleteImageFromS3(image.PreviewKey);
            await _aws.DeleteImageFromS3(image.ThumbnailKey);
            _logger.LogInformation($"Deleted image: {image.FullViewKey}");
        }
        

        return _mapper.Map<ArtworkResponse>(deletedArtwork);
    }


    public async Task<bool> MarkAsSoldAsync(int artworkId)
    {
        _logger.LogInformation("Marking the artworkd {artworkId} as sold", artworkId);

        var artwork = await _repoGet.GetArtworkByIdAsync(artworkId);
        if (artwork == null)
        {
            _logger.LogWarning("The artwork {artworkId} was not found.", artworkId);
            return false;
        }

        if (!artwork.ForSale)
        {
            _logger.LogWarning("Artwork {artworkId} is already marked as sold.", artworkId);
            return false;
        }

        artwork.ForSale = false;
        artwork.Price = null;

        await _repository.UpdateArtworkAsync(artwork);
        return true;
    }
}