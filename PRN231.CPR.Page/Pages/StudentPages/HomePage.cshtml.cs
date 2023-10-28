using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;

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
                    var acc = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/google-authentication?googleId={code}", null,null).Result;
                    if (acc.IsSuccessStatusCode)
                    {
                        string data = await acc.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        var customer = System.Text.Json.JsonSerializer.Deserialize<AccountResponse>(data, options);
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var jwtSecurityToken = tokenHandler.ReadJwtToken(customer.Token);
                        var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                        var role = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "role").Value;
                        identity.AddClaim(new Claim(ClaimTypes.Email, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "email").Value));
                        identity.AddClaim(new Claim(ClaimTypes.Role, jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == "role").Value));
                        var principal = new ClaimsPrincipal(identity);
                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "jwt", customer.Token);
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "role", role);
                        return RedirectToPage("/StudentPages/HomePage");
                    }
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
