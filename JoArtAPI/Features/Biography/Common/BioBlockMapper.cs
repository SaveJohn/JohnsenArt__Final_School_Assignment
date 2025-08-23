using Profile = AutoMapper.Profile;
using JoArtClassLib.About;

namespace JohnsenArtAPI.Features.Biography.Common;

public class BioBlockMapper : Profile
{
    public BioBlockMapper()
    {
        CreateMap<BioBlock, BioBlockRequest>().ReverseMap();
        CreateMap<BioImage, BioImageRequest>().ReverseMap();
        CreateMap<BioBlock, UpdateBioBlockRequest>().ReverseMap();
        CreateMap<BioImage, UpdateBioImageRequest>().ReverseMap();
        CreateMap<BioBlock, BioBlockResponse>().ReverseMap();
        CreateMap<BioImage, BioImageResponse>().ReverseMap();
    }
    
}