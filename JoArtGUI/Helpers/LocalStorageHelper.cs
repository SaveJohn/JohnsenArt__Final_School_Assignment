using JohnsenArtGUI.Helpers.Interfaces;
using Microsoft.JSInterop;

namespace JohnsenArtGUI.Helpers;

public class LocalStorageHelper: ILocalStorageHelper
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _localStorageModule;

    public LocalStorageHelper(IJSRuntime js)
    {
        _js = js;
    }
    
    private async Task<IJSObjectReference> GetModule()
    {
        _localStorageModule ??= await _js.InvokeAsync<IJSObjectReference>("import", "/js/localStorage.js");
        return _localStorageModule;
    }

    public async Task SetItem(string key, string value)
    {
        var module = await GetModule();
        await module.InvokeVoidAsync("setItem", key, value);
    }

    public async Task<string?> GetItem(string key)
    {
        var module = await GetModule();
        return await module.InvokeAsync<string?>("getItem", key);
    }

    public async Task RemoveItem(string key)
    {
        var module = await GetModule();
        await module.InvokeVoidAsync("removeItem", key);
    }

    public async Task Clear()
    {
        var module = await GetModule();
        await module.InvokeVoidAsync("clear");
    }
}