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
 
        private string UrlApi = "";
        public ListTopicModel()
        {
           
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
          
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Something was wrong");
            }

            return Page();
        }
    }
}
