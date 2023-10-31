using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using System.Text.Json;

namespace PRN231.CPR.Page.Pages.TopicPages
{
    public class CreateModel : PageModel
    {
        public string Message { get; set; }
        public async Task<IActionResult> OnPostAsync(IFormFile? file)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
          
            if (token != null && role != null && refreshToken != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Admin"))
                {
                    if (ModelState.IsValid)
                    {
                        if (file != null && file.Length > 0)
                        {
                            HttpResponseMessage response = SendDataHelper<IFormFile>.PostFile($"https://localhost:7298/odata/Topics", file, token).Result;
                            if (response.IsSuccessStatusCode) return RedirectToPage("/SubjectPages/Index");
                            if (response.StatusCode.ToString() == "Unauthorized")
                            {
                                var customer = SendDataHelper<AccountResponse>.GetRefreshToken(token, refreshToken).Result;
                                if (customer != null)
                                {
                                    SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                                    SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                                    return Page();
                                }
                                else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                            }
                            else
                            {
                                var data = response.Content.ReadAsStringAsync().Result;
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                };
                                Message = System.Text.Json.JsonSerializer.Deserialize<ErrorResponse>(data, options).Error;
                                return Page();
                            }
                        }

                    }
                    return Page();
                }
                else return RedirectToPage("/Index");

            }
            else return RedirectToPage("/Index");
        }
    }
}
