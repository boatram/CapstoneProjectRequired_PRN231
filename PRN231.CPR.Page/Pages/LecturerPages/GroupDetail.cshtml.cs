using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;

namespace PRN231.CPR.Page.Pages.LecturerPages
{
    public class GroupDetailModel : PageModel
    {
        public GroupProjectResponse GroupProject { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            if (token != null && refreshToken != null && role != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Lecturer"))
                {
                    if (id == null)
                    {
                        return NotFound();
                    }

                    var groupProject = SendDataHelper<GroupProjectResponse>.GetDataNoOData($"https://localhost:7092/api/GroupProjects/{id}", token).Result;
                    if (groupProject == null)
                    {
                        var customer = SendDataHelper<GroupProjectResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            groupProject = SendDataHelper<GroupProjectResponse>.GetDataNoOData($"https://localhost:7092/api/GroupProjects/{id}", token).Result;
                            if (groupProject != null)
                            {
                                GroupProject = groupProject;
                            }
                            else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                        }
                    }
                    else
                    {
                        GroupProject = groupProject;
                    }
                    return Page();
                }
                else return RedirectToPage("/Index");
            }
            else return RedirectToPage("/Index");
        }
    }
}
