using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using PRN231.CPR.Page.Helpers;
using System.Security.Claims;

namespace PRN231.CPR.Page.Pages.LecturerPages
{
    public class GroupProjectModel : PageModel
    {
        private readonly IConfiguration Configuration;
        public string NameSort { get; set; }
        public string CurrentSort { get; set; }
        public GroupProjectModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public PaginatedList<GroupProjectResponse> GroupProjects { get; set; } = default!;
        public IList<SemesterResponse> Semesters { get; set; } = default!;
        public async Task<IActionResult> OnGet(string sortOrder, int? pageIndex)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            string email = User.FindFirstValue(ClaimTypes.Email);
            List<GroupProjectResponse> list = new List<GroupProjectResponse>();
            this.CurrentSort = sortOrder;
            this.NameSort = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            pageIndex = 1;
            var pageSize = this.Configuration.GetValue("PageSize", 10);
            if (token != null && refreshToken != null && role != null && email != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Lecturer"))
                {
                    var result = SendDataHelper<SemesterResponse>.GetListData($"https://localhost:7298/odata/Semesters", token).Result;
                    if (result != null)
                        Semesters = result.ToList();
                    else Semesters = null;
                    var rs = SendDataHelper<GroupProjectResponse>.GetListDataNoOData($"https://localhost:7092/api/GroupProjects/group-project-lecturer?email={email}", token).Result;//thay api
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
                        this.GroupProjects = PaginatedList<GroupProjectResponse>.Create(list, pageIndex ?? 1, pageSize);
                    }
                    if (list == null)
                    {
                        var customer = SendDataHelper<GroupProjectResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            rs = SendDataHelper<GroupProjectResponse>.GetListDataNoOData($"https://localhost:7092/api/GroupProjects/group-project-lecturer?email={email}", customer.Token).Result;//thay api
                            result = SendDataHelper<SemesterResponse>.GetListData($"https://localhost:7298/odata/Semesters", token).Result;
                            if (rs != null && result != null)
                            {
                                this.GroupProjects = PaginatedList<GroupProjectResponse>.Create(rs.ToList(), pageIndex ?? 1, pageSize);
                                Semesters = result.ToList();
                            }
                            else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                        }
                    }
                    return Page();
                }
                else return RedirectToPage("/Index");
            }
            else return RedirectToPage("/Index");
        }

        public async Task<IActionResult> OnGetSemester(string sortOrder, int? pageIndex, string code)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            string email = User.FindFirstValue(ClaimTypes.Email);
            List<GroupProjectResponse> list = new List<GroupProjectResponse>();
            this.CurrentSort = sortOrder;
            this.NameSort = string.IsNullOrEmpty(sortOrder) ? "id_desc" : "";
            pageIndex = 1;
            var pageSize = this.Configuration.GetValue("PageSize", 10);
            if (token != null && refreshToken != null && role != null && email != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Lecturer"))
                {
                    var result = SendDataHelper<SemesterResponse>.GetListData($"https://localhost:7298/odata/Semesters", token).Result;
                    if (result != null)
                        Semesters = result.ToList();
                    else Semesters = null;
                    var rs = SendDataHelper<GroupProjectResponse>.GetListDataNoOData($"https://localhost:7092/api/GroupProjects/group-project-lecturer-semester?code={code}&email={email}", token).Result;
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
                        this.GroupProjects = PaginatedList<GroupProjectResponse>.Create(list, pageIndex ?? 1, pageSize);
                    }
                    if (list == null)
                    {
                        var customer = SendDataHelper<GroupProjectResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            rs = SendDataHelper<GroupProjectResponse>.GetListDataNoOData($"https://localhost:7092/api/GroupProjects/group-project-lecturer-semester?code={code}&email={email}", customer.Token).Result;
                            result = SendDataHelper<SemesterResponse>.GetListData($"https://localhost:7298/odata/Semesters", token).Result;
                            if (rs != null && result != null)
                            {
                                this.GroupProjects = PaginatedList<GroupProjectResponse>.Create(rs.ToList(), pageIndex ?? 1, pageSize);
                                Semesters = result.ToList();
                            }
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
