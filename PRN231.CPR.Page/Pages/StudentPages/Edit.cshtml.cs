using AutoMapper;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Hangfire.MemoryStorage.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using Repository;
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
            if (token != null)
            {
                    string email = User.FindFirstValue(ClaimTypes.Email);
                    var cus = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$filter=Email eq \'{email}\'", token).Result.SingleOrDefault();
                    if (cus != null)
                    Account = mapper.Map<UpdateAccountRequest>(cus);
                    else return RedirectToPage("/Index");
                    return Page(); 
            }
            else return RedirectToPage("/Index");
        }
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(IFormFile? userDisplayPic)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            if (token != null)
            {
                string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
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
                            if (response.IsSuccessStatusCode)
                            {
                                string data = await response.Content.ReadAsStringAsync();
                                Account.Avatar = data;
                            }
                        }
                        HttpResponseMessage responseMessage = SendDataHelper<UpdateAccountRequest>.PutData($"https://localhost:7298/odata/Accounts/{Account.Id}", Account, token).Result;
                        if (responseMessage.IsSuccessStatusCode) return RedirectToPage("/StudentPages/Edit");
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
                return RedirectToPage("/AdminDashboard");

            }
            else return RedirectToPage("/HomePage");
        }
    }
    }
