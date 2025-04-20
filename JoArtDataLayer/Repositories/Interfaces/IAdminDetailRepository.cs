namespace JoArtDataLayer.Repositories.Interfaces;

public interface IAdminDetailRepository
{
    public Task<string> GetAdminEmail();
}