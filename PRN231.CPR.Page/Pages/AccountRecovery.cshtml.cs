using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.OData.Edm;
using PRN231.CPR.Page.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace PRN231.CPR.Page.Pages
{
    public class AccountRecoveryModel : PageModel
    {
        public string Message { get; set; }
        [BindProperty]
        public ResetPasswordRequest Account { get; set; }
        [BindProperty]
        public string Code { get; set; }
        public void OnGet()
        {
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (Regex.IsMatch(Account.Email, "^[a-zA-Z0-9._%+-]+@(fpt\\.edu\\.vn|fe\\.edu\\.vn|gmail\\.com)$"))
            {
                HttpResponseMessage responseMessage = await SendDataHelper<dynamic>.PostData($"https://localhost:7298/verification?email={Account.Email}", Account.Email);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    var customer = System.Text.Json.JsonSerializer.Deserialize<VerificationResponse>(data, options);
                    if (customer.Success)
                    {
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "verified", true);
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "email", Account.Email);
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "code", customer.Token);
                        return Page();
                    }
                    else SessionHelper.SetObjectAsJson(HttpContext.Session, "verified", false);
                }
                else
                {

                    Message = "Email Not Found";
                    return Page();
                }

            }
            else
            {

                Message = "Your account is not allowed to log into the system";
                return Page();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostVerifiedAsync()
        {
            if (Code !=null)
            {
                string code=SessionHelper.GetObjectFromJson<string>(HttpContext.Session,"code");
                SessionHelper.SetObjectAsJson(HttpContext.Session, "verified", false);
                if (code != null)
                {
                    if (code == Code) SessionHelper.SetObjectAsJson(HttpContext.Session, "equal", true);
                    else SessionHelper.SetObjectAsJson(HttpContext.Session, "equal", false);
                }
            }
            else
            {

                Message = "Please enter code";
                return Page();
            }
            return Page();
        }
        public async Task<IActionResult> OnPostResetAsync()
        {
            if (Account !=null)
            {
                SessionHelper.SetObjectAsJson(HttpContext.Session, "equal", false);
                HttpResponseMessage responseMessage = await SendDataHelper<dynamic>.PostData($"https://localhost:7298/forgotten-password?Email={Account.Email}&NewPassword={Account.NewPassword}&ConfirmNewPassword={Account.ConfirmNewPassword}", Account);
                if (responseMessage.IsSuccessStatusCode) return RedirectToPage("/Index");
                else Message = "Reset Fail !!";
            }
            else
            {

                Message = "Please enter code";
            }
            return Page();
        }
        public async Task<IActionResult> OnPostCancelAsync()
        {
            return RedirectToPage("/Index");
        }
    }
}
