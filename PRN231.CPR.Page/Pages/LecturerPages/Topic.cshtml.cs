using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using PRN231.CPR.Page.Helpers;
using System.Security.Claims;

namespace PRN231.CPR.Page.Pages.LecturerPages
{
    public class TopicModel : PageModel
    {
        private readonly IConfiguration Configuration;
        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public TopicModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public PaginatedList<TopicResponse> Topics { get; set; } = default!;
        public async Task<IActionResult> OnGet(string SearchBy, string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            string email = User.FindFirstValue(ClaimTypes.Email);
            List<TopicResponse> list = new List<TopicResponse>();
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
                if (role.Equals("Lecturer"))
                {
                    refreshToken = refreshToken.Replace(" ", "+");
                    if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(SearchBy))
                    {
                        if (SearchBy.Equals("byname"))
                        {
                            var rs = SendDataHelper<TopicResponse>.GetListData($"https://localhost:7298/odata/TopicView?$filter=SuperLecturerEmail eq \'{email}\'&contains(Name,\'{searchString}\')", token).Result;
                            if (rs != null)
                                list = rs.ToList();
                            else list = null;
                        }
                        else if (SearchBy.Equals("bysemester"))
                        {
                            var rs = SendDataHelper<TopicResponse>.GetListData($"https://localhost:7298/odata/TopicView?$filter=SuperLecturerEmail eq \'{email}\'&contains(SemesterCode,\'{searchString}\')", token).Result;//thay api
                            if (rs != null)
                                list = rs.ToList();
                            else list = null;
                        }
                        else if (SearchBy.Equals("byspecialization"))
                        {
                            var rs = SendDataHelper<TopicResponse>.GetListData($"https://localhost:7298/odata/TopicView?$filter=SuperLecturerEmail eq \'{email}\'&contains(SpecializationName,'{searchString}'", token).Result;// thay api
                            if (rs != null)
                                list = rs.ToList();
                            else list = null;
                        }
                    }
                    else
                    {
                        var rs = SendDataHelper<TopicResponse>.GetListData($"https://localhost:7298/odata/TopicView?$filter=SuperLecturerEmail eq \'{email}\'", token).Result;
                        if (rs != null)
                            list = rs.ToList();
                        else list = null;
                    }
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
                        this.Topics = PaginatedList<TopicResponse>.Create(list, pageIndex ?? 1, pageSize);
                    }
                    if (list == null)
                    {
                        var customer = SendDataHelper<TopicResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            var rs = SendDataHelper<TopicResponse>.GetListData($"https://localhost:7298/odata/TopicView?$filter=SuperLecturerEmail eq \'{email}\'", customer.Token).Result;
                            if (rs != null)
                                this.Topics = PaginatedList<TopicResponse>.Create(rs.ToList(), pageIndex ?? 1, pageSize);
                            else return RedirectToPage("/StudentPages/HomePage", "logout", null);
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
