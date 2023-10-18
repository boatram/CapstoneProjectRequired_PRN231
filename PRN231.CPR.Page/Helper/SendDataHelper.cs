
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
        public static async Task<IList<T>> GetListData(string url)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(url);
            string strData = await responseMessage.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<IList<T>>(strData, options);
        }
        public static async Task<T> GetData(string url)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            HttpResponseMessage responseMessage = await _httpClient.GetAsync(url);
            string strData = await responseMessage.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            return JsonSerializer.Deserialize<T>(strData, options);
        }
        public static async Task<HttpResponseMessage> PostData(string url,T entity)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            string strData = JsonConvert.SerializeObject(entity);
            HttpContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PostAsync(url, content);
            return responseMessage;
        }
        public static async Task<HttpResponseMessage> PutData(string url, T entity)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            string strData = JsonConvert.SerializeObject(entity);
            HttpContent content = new StringContent(strData, Encoding.UTF8, "application/json");
            HttpResponseMessage responseMessage = await _httpClient.PutAsync(url, content);
            return responseMessage;
        }
        public static async Task<HttpResponseMessage> DeleteData(string url)
        {
            HttpClient _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            HttpResponseMessage responseMessage = await _httpClient.DeleteAsync(url);
            return responseMessage;
        }
    }
}
