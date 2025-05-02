using JoArtClassLib;

namespace JoArtDataLayer.Repositories.Interfaces;

public interface IAdminUserRepository
{
    public Task<string> GetAdminEmail();
    
    public Task<Admin> GetAdmin(string? email);
}