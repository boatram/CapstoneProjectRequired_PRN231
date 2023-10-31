using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using System.IdentityModel.Tokens.Jwt;

namespace PRN231.CPR.Page.Pages
{
    public class AdminDashboardModel : PageModel
    {
        public async Task<IActionResult> OnGet()
        {
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string role = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "role");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            if (token != null && refreshToken != null && role != null)
            {
                refreshToken = refreshToken.Replace(" ", "+");
                if (role.Equals("Admin"))
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var jwtSecurityToken = tokenHandler.ReadJwtToken(token);
                    var utcExpiredDate = long.Parse(jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                    var expiredDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiredDate).DateTime;
                    if (expiredDate == DateTime.UtcNow)
                    {
                        var customer = SendDataHelper<AccountResponse>.GetRefreshToken(token, refreshToken).Result;
                        if (customer != null)
                        {
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                            SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                            return Page();
                        }
                        else return RedirectToPage("/StudentPages/HomePage", "logout", null);
                    }
                }
                return Page();
            }
            else return RedirectToPage("/Index");
        }
    }
}
