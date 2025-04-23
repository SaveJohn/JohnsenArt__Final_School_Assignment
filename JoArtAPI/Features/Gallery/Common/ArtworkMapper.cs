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
        CreateMap<ArtworkImage, ImageRequest>().ReverseMap();
        CreateMap<Artwork, UpdateArtworkRequest>().ReverseMap();
        CreateMap<ArtworkImage, UpdateImageRequest>().ReverseMap();
        CreateMap<Artwork, ArtworkResponse>().ReverseMap();
        CreateMap<ArtworkImage, ImageResponse>().ReverseMap();
        CreateMap<Neighbors, NeighborsResponse>();
        
    }
}