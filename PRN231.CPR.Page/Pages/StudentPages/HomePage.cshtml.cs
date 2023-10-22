using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;

namespace PRN231.CPR.Page.Pages.StudentPages
{
    public class HomePageModel : PageModel
    {
        public async Task<IActionResult> OnGet(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var acc = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/google-authentication?googleId={code}", null).Result;
                    if (acc.IsSuccessStatusCode)
                        return RedirectToPage("/StudentPages/HomePage");
                    else
                    {
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "ERROR", "Your account is not allowed to log into the system");
                        return RedirectToPage("/Index");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Page();
        }
    }
}
