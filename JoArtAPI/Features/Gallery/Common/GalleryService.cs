using AutoMapper;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using JoArtClassLib.Enums;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace JohnsenArtAPI.Features.Gallery.Common;

public class GalleryService : IGalleryService
{
    private readonly IGalleryRepository _repository;
    private readonly IAwsService _aws;
    private readonly IMapper _mapper;
    private readonly ILogger<GalleryService> _logger;

    public GalleryService(
        IGalleryRepository repository,
        IAwsService aws,
        IMapper mapper,
        ILogger<GalleryService> logger)
    {
        _repository = repository;
        _aws = aws;
        _mapper = mapper;
        _logger = logger;
    }

    // Get all artworks
    public async Task<IEnumerable<ArtworkResponse?>> GetArtworksAsync(
        int page, int perPage, GallerySort sort, GalleryFilter filter)
    {
        _logger.LogInformation($"-------------------- \n Service: GetArtworks:");

        // Get artworks from repository and map to response DTO
        var responses = _mapper.Map<List<ArtworkResponse?>>(
            await _repository.GetArtworksAsync(page, perPage, sort, filter));

        // No artworks found
        if (responses.Count == 0)
        {
            _logger.LogWarning("No artworks found");
            return responses;
        }

        // Set pre siged ImageUrl for each image (clearer, avoids nested lambdas)
        foreach (var response in responses)
        {
            if (response != null)
            {
                foreach (var image in response.Images)
                {
                    image.ImageUrl = _aws.GeneratePresignedUrl(image.ObjectKey);
                    image.PreviewUrl = _aws.GeneratePresignedUrl(image.PreviewKey);
                    image.ThumbnailUrl = _aws.GeneratePresignedUrl(image.ThumbnailKey);
                }

                _logger.LogInformation($"Found {response.Images.Count} images for artwork '{response.Title}'");
            }
        }

        return responses.AsEnumerable();
    }


    // Get Artwork By ID
    public async Task<ArtworkResponse?> GetArtworkByIdAsync(int artId)
    {
        _logger.LogInformation($"-------------------- \n Service: GetArtworkById {artId}:");

        // Getting artwork from database (mapped to response)
        var response = _mapper.Map<ArtworkResponse>(
            await _repository.GetArtworkByIdAsync(artId)
        );

        // No artwork found
        if (response == null)
        {
            _logger.LogWarning($"Art {artId} not found");
            return response;
        }

        // Setting Pre-Singed Url for each image
        foreach (var image in response.Images)
        {
            image.ImageUrl = _aws.GeneratePresignedUrl(image.ObjectKey);
            image.PreviewUrl = _aws.GeneratePresignedUrl(image.PreviewKey);
            image.ThumbnailUrl = _aws.GeneratePresignedUrl(image.ThumbnailKey);
        }

        return response;
    }

    public async Task<NeighborsResponse> GetGalleryNeighborsAsync(int artId, GallerySort sort, GalleryFilter filter)
    {
       var neighbors = await _repository.GetGalleryNeighborsAsync(artId, sort, filter);
       
       return _mapper.Map<NeighborsResponse>(neighbors);
       
    }

    public async Task<IEnumerable<string?>> GetRotationUrls()
    {
        List<string> keys = (List<string>)await _repository.GetRotationObjectKeys();
        List<string> urls = new ();
        
        if (keys.IsNullOrEmpty()) return urls;
        
        foreach (var key in keys)
        {
            urls.Add(_aws.GeneratePresignedUrl(key));
        }
        return urls;
    }
}