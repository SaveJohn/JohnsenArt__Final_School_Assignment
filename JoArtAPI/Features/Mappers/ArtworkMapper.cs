
using JoArtClassLib;
using JoArtClassLib.Art;
using Profile = AutoMapper.Profile;

public class ArtworkMapper : Profile
{
    public ArtworkMapper()
    {
        CreateMap<Artwork, ArtworkDTO>().ReverseMap();
        CreateMap<ArtworkImage, ArtworkImageDTO>().ReverseMap();
    }
}