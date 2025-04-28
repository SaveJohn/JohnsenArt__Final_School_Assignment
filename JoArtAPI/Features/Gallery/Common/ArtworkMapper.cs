using JoArtClassLib;
using JoArtClassLib.Art;
using JoArtClassLib.Art.Artwork;
using Profile = AutoMapper.Profile;

namespace JohnsenArtAPI.Features.Gallery.Common;

public class ArtworkMapper : Profile
{
    public ArtworkMapper()
    {
        
        CreateMap<Artwork, ArtworkRequest>().ReverseMap();
        CreateMap<Image, ImageRequest>().ReverseMap();
        CreateMap<Artwork, UpdateArtworkRequest>().ReverseMap();
        CreateMap<Image, UpdateImageRequest>().ReverseMap();
        CreateMap<Artwork, ArtworkResponse>().ReverseMap();
        CreateMap<Image, ImageResponse>().ReverseMap();
        CreateMap<Neighbors, NeighborsResponse>();
        
    }
}