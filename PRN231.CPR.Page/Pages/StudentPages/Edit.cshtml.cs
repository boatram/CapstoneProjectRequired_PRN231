using AutoMapper;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using Repository;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;

namespace PRN231.CPR.Page.Pages.StudentPages
{
    public class EditModel : PageModel
    {
        [BindProperty]
        public UpdateAccountRequest Account { get; set; } = default!;
        public IMapper mapper { get; set; }
        public string Message { get; set; }
        public EditModel(IMapper mapper)
        {
            this.mapper = mapper;
        }
        public async Task<IActionResult> OnGet()
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
          
            if (token != null && refreshToken !=null && role !=null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Student") || role.Equals("Lecturer"))
                {
                    string email = User.FindFirstValue(ClaimTypes.Email);
                    var cus = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$filter=Email eq \'{email}\'", token).Result;
                    if (cus != null)
                        Account = mapper.Map<UpdateAccountRequest>(cus.SingleOrDefault());
                    else
                    {
                        HttpResponseMessage responseMessage = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/token-verification?Token={token}&RefreshToken={refreshToken}",null, null).Result;
                        if (responseMessage.IsSuccessStatusCode)
                        {
                            string data = await responseMessage.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            var customer = System.Text.Json.JsonSerializer.Deserialize<AccountResponse>(data, options);
                            Account = mapper.Map<UpdateAccountRequest>(customer);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                        }
                        else return RedirectToPage("/Index");
                    }
                    return Page();
                }
                return RedirectToPage("/AdminDashboard");

            }
            else return RedirectToPage("/Index");
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? userDisplayPic)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
           
            if (token != null && role != null && refreshToken != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Student") || role.Equals("Lecturer"))
                {
                    if (ModelState.IsValid)
                    {
                        if (Account.DateOfBirth > DateTime.UtcNow)
                        {
                            Message = "Invalid Date";
                            return Page();
                        }
                        if (userDisplayPic != null && userDisplayPic.Length > 0)
                        {
                            HttpResponseMessage response = SendDataHelper<IFormFile>.PostFile($"https://localhost:7298/api/files", userDisplayPic, token).Result;
                            if (response.IsSuccessStatusCode) Account.Avatar = await response.Content.ReadAsStringAsync();
                            else
                            {
                                HttpResponseMessage responseMes = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/token-verification?Token={token}&RefreshToken={refreshToken}", null, null).Result;
                                if (responseMes.IsSuccessStatusCode)
                                {
                                    string data = await responseMes.Content.ReadAsStringAsync();
                                    var options = new JsonSerializerOptions
                                    {
                                        PropertyNameCaseInsensitive = true,
                                    };
                                    var customer = System.Text.Json.JsonSerializer.Deserialize<AccountResponse>(data, options);
                                    Account = mapper.Map<UpdateAccountRequest>(customer);
                                    SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                                    SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                                    RedirectToPage("/StudentPages/Edit");
                                }
                                else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                            }
                        }
                        HttpResponseMessage responseMessage = SendDataHelper<UpdateAccountRequest>.PutData($"https://localhost:7298/odata/Accounts/{Account.Id}", Account, token).Result;
                        if (responseMessage.IsSuccessStatusCode) return RedirectToPage("/StudentPages/Edit");
                        if(responseMessage.StatusCode.ToString()== "Unauthorized")
                        {
                            HttpResponseMessage responseMes = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/token-verification?Token={token}&RefreshToken={refreshToken}", null, null).Result;
                            if (responseMes.IsSuccessStatusCode)
                            {
                                string data = await responseMes.Content.ReadAsStringAsync();
                                var options = new JsonSerializerOptions
                                {
                                    PropertyNameCaseInsensitive = true,
                                };
                                var customer = System.Text.Json.JsonSerializer.Deserialize<AccountResponse>(data, options);
                                Account = mapper.Map<UpdateAccountRequest>(customer);
                                SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                                SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                                RedirectToPage("/StudentPages/Edit");
                            }
                            else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                        }
                        else
                        {

                            var data = responseMessage.Content.ReadAsStringAsync().Result;
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            Message = System.Text.Json.JsonSerializer.Deserialize<ErrorResponse>(data, options).Error;
                            return Page();
                        }
                       
                    }
                    return Page();
                }
               else return RedirectToPage("/AdminDashboard");

            }
            else return RedirectToPage("/Index");
        }
    }
    }
