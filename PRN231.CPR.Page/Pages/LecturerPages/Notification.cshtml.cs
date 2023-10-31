using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using PRN231.CPR.Page.Helpers;
using System.Drawing.Printing;
using System.Security.Claims;
using System.Text.Json;

namespace PRN231.CPR.Page.Pages.LecturerPages
{
    public class NotificationModel : PageModel
    {
        private readonly IConfiguration Configuration;
        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public NotificationModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public PaginatedList<GroupProjectResponse> GroupProjectResponses { get; set; } = default!;
        public async Task<IActionResult> OnGet(string SearchBy, string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            string email = User.FindFirstValue(ClaimTypes.Email);
            List<GroupProjectResponse> list = new List<GroupProjectResponse>();
           
            this.CurrentSort = sortOrder;
            this.NameSort = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";

            if (searchString != null)
            {
                pageIndex = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            this.CurrentFilter = searchString;
            var pageSize = this.Configuration.GetValue("PageSize", 4);
            if (token != null && refreshToken != null && role != null && email !=null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Lecturer"))
                {
                        var rs = SendDataHelper<GroupProjectResponse>.GetListDataNoOData($"https://localhost:7092/api/GroupProjects/topic-of-group-lecturer?code={email}", token).Result;
                        if (rs != null)
                            list = rs.ToList();
                        else list = null;
                    if (list != null && list.Count() != 0)
                    {
                        switch (sortOrder)
                        {
                            case "id_desc":
                                list = list.OrderByDescending(a => a.Id).ToList();
                                break;
                            default:
                                list = list.OrderBy(a => a.Id).ToList();
                                break;
                        }
                        this.GroupProjectResponses = PaginatedList<GroupProjectResponse>.Create(list, pageIndex ?? 1, pageSize);
                        
                    }
                    if (list == null)
                    {
                        var customer = SendDataHelper<AccountResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            rs = SendDataHelper<GroupProjectResponse>.GetListData($"https://localhost:7092/api/GroupProjects/topic-of-group-lecturer?code={email}", customer.Token).Result;
                            if (rs != null)
                                this.GroupProjectResponses = PaginatedList<GroupProjectResponse>.Create(rs.ToList(), pageIndex ?? 1, pageSize);
                            else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                        }
                    }
                    return Page();
                }
                else return RedirectToPage("/Index");
            }
            else return RedirectToPage("/Index");

        }
        public IActionResult OnGetUpdate(int groupId, int topicId)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");

            if (token != null && role != null && refreshToken != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Lecturer"))
                {
                    if (ModelState.IsValid)
                    {
                        if (groupId != null && topicId !=null)
                        {
                            HttpResponseMessage response = SendDataHelper<IFormFile>.PutData($" https://localhost:7298/accept-topic?topicId={topicId}&groupId={groupId}", null, token).Result;
                            if (response.IsSuccessStatusCode) return RedirectToPage("/LecturerPages/Notification");
                            if (response.StatusCode.ToString() == "Unauthorized")
                            {
                                var customer = SendDataHelper<AccountResponse>.GetRefreshToken(token, refreshToken).Result;
                                if (customer != null)
                                {
                                    SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                                    SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                                    response = SendDataHelper<IFormFile>.PutData($" https://localhost:7298/accept-topic?topicId={topicId}&groupId={groupId}", null, token).Result;
                                    if (response.IsSuccessStatusCode) return Page();
                                    else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                                }
                                else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                            }
                       
                        }

                    }
                    return RedirectToPage("/LecturerPages/Notification");
                }
                else return RedirectToPage("/Index");

            }
            else return RedirectToPage("/Index");
        }

    }
}
