using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArchiBase.Utils;

public static class HttpClientExtensions
{
    public static readonly JsonSerializerOptions SerializerOptionsOutput = new(JsonSerializerDefaults.Web)
    {
        ReferenceHandler = ReferenceHandler.Preserve,
    };

    public static async Task<T?> GetFromJsonAsyncExtended<T>(this HttpClient client, string url)
    {
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode)
        {
            return default;
        }
        var result = await response.Content.ReadFromJsonAsync<T>(SerializerOptionsOutput);
        //result ??= await response.Content.ReadFromJsonAsync<T>();
        return result;
    }

    public static async Task<HttpResponseMessage> PostAsJsonAsyncExtended<T>(this HttpClient client, string url, T data)
    {
        var result = await client.PostAsJsonAsync(url, data, SerializerOptionsOutput);
        return result;
    }

    public static async Task<HttpResponseMessage> PutAsJsonAsyncExtended<T>(this HttpClient client, string url, T data)
    {
        var result = await client.PutAsJsonAsync(url, data, SerializerOptionsOutput);
        return result;
    }
}