using AutoMapper;
using JoArtClassLib.Art;
using JoArtDataLayer.Repositories.Interfaces;
using JohnsenArtAPI.Features.Gallery.Aws.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Interfaces;

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
    
    // Get All Artworks
    public async Task<IEnumerable<ArtworkResponse?>> GetArtworksAsync(int page, int perPage, bool? newest, bool? forSale)
    {
        _logger.LogInformation($"-------------------- \n Service: GetArtworks:");
        var responses = _mapper.Map<List<ArtworkResponse>>(
            await _repository.GetArtworksAsync(page, perPage, newest, forSale));
        
        // No artworks found
        if (responses.Count == 0)
        {
            _logger.LogWarning($"No artworks found");
            return responses;
        }
        
        // Setting Pre-Singed Url for each image
        responses.ForEach(response => 
            response.Images.ForEach(image => 
                image.ImageUrl = _aws.GeneratePresignedUrl(image.ObjectKey)));

        
        return responses;
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
            var imageUrl = _aws.GeneratePresignedUrl(image.ObjectKey);
            image.ImageUrl = imageUrl;
        }
        
        return response;
    }
}