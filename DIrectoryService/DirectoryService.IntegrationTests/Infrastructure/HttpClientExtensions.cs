using System.Net.Http.Json;

namespace DirectoryService.IntegrationTests.Infrastructure;

public static class HttpClientExtensions
{
    public static async Task<T> PostAndReadAsJsonAsync<T, TValue>(this HttpClient client, string url, TValue value)
    {
        var response = await client.PostAsJsonAsync(url, value);
        return await response.Content.ReadFromJsonAsync<T>();
    }
    
    public static async Task<T> PutAndReadAsJsonAsync<T, TValue>(this HttpClient client, string url, TValue value)
    {
        var response = await client.PutAsJsonAsync(url, value);
        return await response.Content.ReadFromJsonAsync<T>();
    }
}