using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using BusinessObjects.BusinessObjects;
using AutoMapper;
using BusinessObjects.DTOs.Request;

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
                        SessionHelper.SetObjectAsJson(HttpContext.Session, "refreshToken", customer.RefreshToken);
                        string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
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
        public async Task<IActionResult> OnGetLogout()
        {
            var scheme = CookieAuthenticationDefaults.AuthenticationScheme;
            string email = User.FindFirstValue(ClaimTypes.Email);
            string token = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "jwt");
            string refreshToken = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "refreshToken");
            if (token != null && refreshToken != null)
            {
                HttpResponseMessage responseMessage = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/token-revoke?email={email}", null, token).Result;
                if (responseMessage.IsSuccessStatusCode)
                {
                    await HttpContext.SignOutAsync(scheme);
                    Response.Cookies.Delete(scheme);
                    SessionHelper.Delete(HttpContext.Session, "jwt");
                    SessionHelper.Delete(HttpContext.Session, "role");
                    SessionHelper.Delete(HttpContext.Session, "refreshToken");
                }
                if (responseMessage.StatusCode.ToString() == "Unauthorized")
                {
                    HttpResponseMessage responseMes = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/token-verification?Token={token}&RefreshToken={refreshToken}", null, null).Result;
                    if (responseMes.IsSuccessStatusCode)
                    {
                        string data = await responseMes.Content.ReadAsStringAsync();
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        var customer = System.Text.Json.JsonSerializer.Deserialize<AccountResponse>(data, options);
                        HttpResponseMessage response = SendDataHelper<AccountResponse>.PostData($"https://localhost:7298/token-revoke?email={email}", null, customer.Token).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            await HttpContext.SignOutAsync(scheme);
                            Response.Cookies.Delete(scheme);
                            SessionHelper.Delete(HttpContext.Session, "jwt");
                            SessionHelper.Delete(HttpContext.Session, "role");
                            SessionHelper.Delete(HttpContext.Session, "refreshToken");
                        }
                    }
                }
                return RedirectToPage("/Index");
            }
            return RedirectToPage("/Index");

        }
    }
}
