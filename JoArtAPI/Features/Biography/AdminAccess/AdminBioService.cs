using AutoMapper;
using JoArtClassLib.About;
using JoArtDataLayer.Repositories.Biography.Interfaces;
using JohnsenArtAPI.Features.Biography.AdminAccess.Interfaces;
using JohnsenArtAPI.Features.Gallery.Common.Aws.Interfaces;

namespace JohnsenArtAPI.Features.Biography.AdminAccess;

public class AdminBioService : IAdminBioService
{
    private readonly ILogger<AdminBioService> _logger;
    private readonly IMapper _mapper;
    private readonly IAdminBioRepository _repository;
    private readonly IBioRepository _repoGet;
    private readonly IAwsService _aws;

    public AdminBioService(
        ILogger<AdminBioService> logger,
        IMapper mapper,
        IAdminBioRepository repository,
        IBioRepository repoGet,
        IAwsService aws)
    {
        _logger = logger;
        _mapper = mapper;
        _repository = repository;
        _repoGet = repoGet;
        _aws = aws;
    }


    public Task<BioBlock?> UploadBioBlockAsync(BioBlockRequest? bioBlock)
    {
        throw new NotImplementedException();
    }

    public Task<BioBlock> UpdateBioBlockAsync(UpdateBioBlockRequest bioBlock)
    {
        throw new NotImplementedException();
    }

    public Task<BioBlock> DeleteBioBlockAsync(int bioBlockId)
    {
        throw new NotImplementedException();
    }
}