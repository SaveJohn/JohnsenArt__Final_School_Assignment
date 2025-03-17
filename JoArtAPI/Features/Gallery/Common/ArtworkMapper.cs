
using JoArtClassLib;
using JoArtClassLib.Art;
using Profile = AutoMapper.Profile;

public class ArtworkMapper : Profile
{
    public ArtworkMapper()
    {
        
        CreateMap<Artwork, ArtworkRequest>().ReverseMap();
        CreateMap<ArtworkImage, ImageRequest>().ReverseMap();
        CreateMap<Artwork, ArtworkResponse>().ReverseMap();
        CreateMap<ArtworkImage, ImageResponse>().ReverseMap();
        
    }
}