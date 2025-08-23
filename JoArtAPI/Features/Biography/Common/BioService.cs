using AutoMapper;
using JoArtDataLayer.Repositories.Biography.Interfaces;
using JohnsenArtAPI.Features.Biography.Common.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;

namespace JohnsenArtAPI.Features.Biography.Common;

public class BioService : IBioService
{
    private readonly ILogger<BioService> _logger;
    private readonly IBioRepository _repository;
    private readonly IAwsService _aws;
    private readonly IMapper _mapper;

    public BioService(
        ILogger<BioService> logger,
        IBioRepository repository,
        IAwsService aws,
        IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _aws = aws;
        _mapper = mapper;
    }
    
    // Get bio block
}