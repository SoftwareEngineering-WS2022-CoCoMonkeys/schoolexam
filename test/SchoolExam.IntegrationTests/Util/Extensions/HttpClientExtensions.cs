using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace SchoolExam.IntegrationTests.Util.Extensions;

public static class HttpClientExtensions
{
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data)
    {
        var json = JsonConvert.SerializeObject(data);
        var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        return httpClient.PostAsync(requestUri, stringContent);
    }
    
    public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data, JsonSerializerOptions options)
    {
        var json = JsonSerializer.Serialize(data, options);
        var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        return httpClient.PostAsync(requestUri, stringContent);
    }
    
    public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string requestUri, T data)
    {
        var json = JsonConvert.SerializeObject(data);
        var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);

        return httpClient.PutAsync(requestUri, stringContent);
    }
}