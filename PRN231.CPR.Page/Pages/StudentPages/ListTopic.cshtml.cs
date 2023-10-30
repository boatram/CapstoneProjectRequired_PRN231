using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using PRN231.CPR.Page.Helper;
using System.Net.Http.Headers;

namespace PRN231.CPR.Page.Pages.StudentPages
{
    public class ListTopicModel : PageModel
    {
        public List<TopicResponse> Topics { get; set; } = new List<TopicResponse>();
        private readonly HttpClient _httpClient = null;
        private string UrlApi = "";
        public ListTopicModel()
        {
            _httpClient = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            _httpClient.DefaultRequestHeaders.Accept.Add(contentType);
            UrlApi = "https://localhost:7298/odata/Topics";
        }
        public async Task<IActionResult> OnGet()
        {
  
            try
            {
                var topics = SendDataHelper<TopicResponse>.GetListData(UrlApi,null).Result;
                foreach(TopicResponse topic in topics)
                {
                    Topics.Add(topic);
                }
               // HttpResponseMessage response = await _httpClient.GetAsync(UrlApi);
              //  if (response.IsSuccessStatusCode)
               // {
               //     var responseContent = await response.Content.ReadAsStringAsync();
              //      Topics = JsonConvert.DeserializeObject<List<TopicResponse>>(responseContent);

              //  }
               // else
               // {
                //    ModelState.AddModelError(string.Empty, "Failed to get topic information");
              //      return NotFound();
              //  }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something was wrong");
            }

            return Page();
        }
    }
}
