using BusinessObjects.DTOs.Request;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PRN231.CPR.Page.Helper;
using System.Security.Claims;
using System.Text.Json;
using BusinessObjects.DTOs.Response;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Text.RegularExpressions;

namespace PRN231.CPR.Page.Pages
{
    public class IndexModel : PageModel
    {
        public string Message { get; set; }
        [BindProperty]
        public LoginRequest Account { get; set; }

        public IndexModel()
        {
            
        }
        public async Task<IActionResult> OnGet(string code)
        {
            var s = SessionHelper.GetObjectFromJson<string>(HttpContext.Session, "ERROR");
            if(s != null) Message = s;
            return Page();
        }
        public IActionResult OnGetGoogle(string email)
        {
           return RedirectPermanent(GoogleApiHelper.GetOauthUri(email));
            
        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid && Regex.IsMatch(Account.Email, "^[a-zA-Z0-9._%+-]+@(fpt\\.edu\\.vn|fe\\.edu\\.vn|gmail\\.com)$"))
            {
                HttpResponseMessage responseMessage = await SendDataHelper<LoginRequest>.PostData($"https://localhost:7298/authentication", Account,null);
                if (responseMessage.IsSuccessStatusCode)
                {
                    string data = await responseMessage.Content.ReadAsStringAsync();
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
                    if (role.Equals("admin"))
                        return RedirectToPage("/AdminDashboard");
                    else
                        return RedirectToPage("/StudentPages/HomePage");
                }
                else
                {

                    Message = "Invalid username or password!";
                    return Page();
                }

            }
            else
            {

                Message = "Your account is not allowed to log into the system";
                return Page();
            }
           
        }
    }
}