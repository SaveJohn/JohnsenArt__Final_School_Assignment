using AutoMapper;
using JoArtDataLayer.Repositories.Biography.Interfaces;
using JohnsenArtAPI.Features.Biography.Common.Aws.Interfaces;
using JohnsenArtAPI.Features.Biography.Common.Interfaces;

namespace JohnsenArtAPI.Features.Biography.Common;

public class BioService : IBioService
{
    private readonly ILogger<BioService> _logger;
    private readonly IBioRepository _repository;
    private readonly IBioAwsService _aws;
    private readonly IMapper _mapper;

    public BioService(
        ILogger<BioService> logger,
        IBioRepository repository,
        IBioAwsService aws,
        IMapper mapper)
    {
        _logger = logger;
        _repository = repository;
        _aws = aws;
        _mapper = mapper;
    }
    
    // Get bio block
}