using AutoMapper;
using BusinessObjects.DTOs.Request;
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
    public class IndexModel : PageModel
    {
        private readonly IConfiguration Configuration;
        public string NameSort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        public IndexModel(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public PaginatedList<AccountResponse> Account { get; set; } = default!;
        public async Task<IActionResult> OnGet(string SearchBy, string sortOrder, string currentFilter, string searchString, int? pageIndex)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            List<AccountResponse> list = new List<AccountResponse>();
           
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
            if (token != null && refreshToken != null && role != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Admin"))
                {

                    if (!string.IsNullOrEmpty(searchString) && !string.IsNullOrEmpty(SearchBy))
                    {
                        if (SearchBy.Equals("byname"))
                        {
                            var rs = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$expand=Specialization&$filter=RoleId eq 2 and contains(Name,\'{searchString}\')", token).Result;
                            if (rs != null)
                                list = rs.ToList();
                            else list = null;
                        }
                        else if (SearchBy.Equals("byid"))
                        {
                            var rs = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$expand=Specialization&$filter=RoleId eq 2 and contains(Code,\'{searchString}\')", token).Result;
                            if (rs != null)
                                list = rs.ToList();
                            else list = null;
                        }
                        else if (SearchBy.Equals("byemail"))
                        {
                            var rs = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$expand=Specialization&$filter=RoleId eq 2 and contains(Email,\'{searchString}\')", token).Result;
                            if (rs != null)
                                list = rs.ToList();
                            else list = null;
                        }
                    }
                    else
                    {
                        var rs = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$expand=Specialization&$filter=RoleId eq 2", token).Result;
                        if (rs != null)
                            list = rs.ToList();
                        else list = null;
                    }
                    if (list != null && list.Count() != 0)
                    {
                        switch (sortOrder)
                        {
                            case "id_desc":
                                list = list.OrderByDescending(a => a.Code).ToList();
                                break;
                            default:
                                list = list.OrderBy(a => a.Code).ToList();
                                break;
                        }
                        this.Account = PaginatedList<AccountResponse>.Create(list, pageIndex ?? 1, pageSize);
                    }
                    if(list==null)
                    {
                        var customer = SendDataHelper<AccountResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            var rs = SendDataHelper<AccountResponse>.GetListData($"https://localhost:7298/odata/Accounts/?$expand=Specialization&$filter=RoleId eq 2", customer.Token).Result;
                            if (rs != null)
                                this.Account = PaginatedList<AccountResponse>.Create(rs.ToList(), pageIndex ?? 1, pageSize);
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
