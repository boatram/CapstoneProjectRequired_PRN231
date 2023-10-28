
using Firebase.Auth;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NuGet.Protocol;
using System.Collections;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace PRN231.CPR.Page.Helper
{
    public static class SendDataHelper<T> where T : class
    {
        public static async Task<IList<T>> GetListData(string url, string? token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode.ToString() == "Unauthorized")
                return null;
            string strData = await response.Content.ReadAsStringAsync();
            dynamic tmp = JsonConvert.DeserializeObject(strData);
            string data = JsonConvert.SerializeObject(tmp.value);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            if (response.StatusCode.ToString() != "Forbidden")
                return JsonSerializer.Deserialize<IList<T>>(data, options);
            else return null;
        }
        public static async Task<T> GetData(string url, string? token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode.ToString() == "Unauthorized")
                return null;
            string strData = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            if (response.StatusCode.ToString() != "Forbidden")
                return JsonSerializer.Deserialize<T>(strData, options);
            else return null;
        }
        public static async Task<HttpResponseMessage> PostData(string url, T entity, string? token)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string strData = JsonConvert.SerializeObject(entity);
            HttpContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(url, content);
            return responseMessage;
        }
        public static async Task<HttpResponseMessage> PostFile(string url, IFormFile entity, string? token)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("multipart/form-data");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            using var requestContent = new MultipartFormDataContent();
            requestContent.Add(new StreamContent(entity.OpenReadStream()), "file",entity.FileName);
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(url, requestContent);
            return responseMessage;
        }
        public static async Task<HttpResponseMessage> PutData(string url, T entity, string? token)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            string strData = JsonConvert.SerializeObject(entity);
            HttpContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PutAsync(url, content);
            return responseMessage;
        }
        public static async Task<HttpResponseMessage> DeleteData(string url, string? token)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(url);
            return responseMessage;
        }
    }
}
