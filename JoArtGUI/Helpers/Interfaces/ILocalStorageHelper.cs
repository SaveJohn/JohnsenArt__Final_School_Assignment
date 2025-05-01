namespace JohnsenArtGUI.Helpers.Interfaces;

public interface ILocalStorageHelper
{
    Task SetItem(string key, string value);
    Task<string?> GetItem(string key);
    Task RemoveItem(string key);
    Task Clear();

}