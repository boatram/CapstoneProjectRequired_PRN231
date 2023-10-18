using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using System.Security.Claims;
using System.Text.Json;

namespace PRN231.CPR.Page.Pages
{
    public class HomePageModel : PageModel
    {
   
        public async Task<IActionResult> OnGet(string code)
        {
            try
            {
                if (!string.IsNullOrEmpty(code))
                {
                    var acc = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/google-authentication?googleId={code}",null).Result;
                    if (acc.IsSuccessStatusCode)
                        return RedirectToPage("./HomePage");
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
