
using AutoMapper;
using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Firebase.Auth;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using OfficeOpenXml;
using Repository.Exceptions;
using Repository.Extensions;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Repository.Helpers.Enum;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Repository
{
    public class AccountRepository : IAccountRepository
    {
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly ICacheService cacheService;
        private readonly ISpecializationRepository specializationRepository;
        public AccountRepository( IMapper mapper, IConfiguration configuration,ISpecializationRepository specializationRepository,ICacheService cacheService) 
        { 
            _mapper = mapper;
            _config = configuration;
            this.specializationRepository = specializationRepository;
            this.cacheService = cacheService;
        }
        public async Task<List<AccountResponse>> CreateAccount(IFormFile file, ExcelChoice choice)
        {
            try
            {
                List<AccountRequest> list = new List<AccountRequest>();
                if (file != null)
                {
                        using (ExcelPackage package = new ExcelPackage(file.OpenReadStream()))
                        {
                            ExcelWorkbook workbook = package.Workbook;
                            if (workbook != null)
                            {
                                ExcelWorksheet worksheet = workbook.Worksheets.FirstOrDefault();
                                if (worksheet != null)
                                {
                                    list = worksheet.ReadExcelToList<AccountRequest>();
                                    foreach (AccountRequest account in list)
                                    {
                                    var customer = _mapper.Map<AccountRequest, Account>(account);
                                    var s = AccountDAO.Instance.GetAccounts().Where(s => s.Code == account.Code).SingleOrDefault();
                                    if (s != null)
                                    {
                                        throw new CrudException(HttpStatusCode.BadRequest, "Code has already !!!", "");
                                    }
                                    var cus = AccountDAO.Instance.GetAccounts().Where(s => s.Email == account.Email || s.Email.Equals(_config["AdminAccount:Email"])).SingleOrDefault();
                                    if (cus != null)
                                    {
                                        throw new CrudException(HttpStatusCode.BadRequest, "Email has already !!!", "");
                                    }
                                    if(!Regex.IsMatch(account.Email, "^[a-zA-Z0-9._%+-]+@(fpt\\.edu\\.vn|fe\\.edu\\.vn|gmail\\.com)$"))
                                        throw new CrudException(HttpStatusCode.BadRequest, "Email invalid !!!", "");
                                    #region checkPhone
                                    var checkPhone = CheckVNPhone(account.Phone);
                                    if (!checkPhone)                                    
                                    {
                                        throw new CrudException(HttpStatusCode.BadRequest, "Wrong Phone", "");
                                    }
                                    #endregion
                                    customer.Status = true;
                                    if (choice.ToString().Equals("Student"))
                                        customer.RoleId = 1;                                                                             
                                    else
                                    {   
                                        customer.RoleId = 2;
                                        CreatPasswordHash("1", out byte[] passwordHash, out byte[] passwordSalt);
                                        customer.PasswordHash = passwordHash;
                                        customer.PasswordSalt = passwordSalt;
                                    }
                                    var spec = specializationRepository.GetSpecializations().Where(a => a.Code.Equals(account.SpecializationCode)).SingleOrDefault();
                                    if (spec == null)
                                    {
                                        throw new CrudException(HttpStatusCode.NotFound, "Specialization Not Found !!!", "");
                                    }
                                    customer.SpecializationId = spec.Id;
                                    AccountDAO.Instance.AddNew(customer);
                                        
                                        
                                    }
                                return _mapper.Map<List<AccountResponse>>(list.ToList());
                            }
                            }
                        }
                }
                return null;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create Account Error!!!", ex?.Message);
            }
        }
        public static bool CheckVNPhone(string phoneNumber)
        {
            string strRegex = @"(^(0)(3[2-9]|5[6|8|9]|7[0|6-9]|8[0-6|8|9]|9[0-4|6-9])[0-9]{7}$)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(phoneNumber))
            {
                return true;
            }
            else
                return false;
        }
        public async Task<AccountResponse> LoginGoogle(string code)
        {
            GoogleToken token = null;

            var postData = new
            {
                code = code,
                client_id = _config["Google:ClientId"],
                client_secret = _config["Google:ClientSecret"],
                redirect_uri = _config["RedirectUri"],
                grant_type = "authorization_code",
            };

            using (var httpClient = new HttpClient())
            {
                StringContent content = new StringContent(JsonConvert.SerializeObject(postData), Encoding.UTF8, "application/json");

                using (var response = await httpClient.PostAsync(_config["TokenUri"], content))
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        string responseString = await response.Content.ReadAsStringAsync();
                        token = JsonConvert.DeserializeObject<GoogleToken>(responseString);
                        if(token != null)
                        {
                            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
                            httpClient.DefaultRequestHeaders.Accept.Add(contentType);
                            var link = _config["GetUserInfor"] + token.access_token;
                            HttpResponseMessage responseMessage = await httpClient.GetAsync(link);
                            string strData = await responseMessage.Content.ReadAsStringAsync();
                            var options = new JsonSerializerOptions
                            {
                                PropertyNameCaseInsensitive = true,
                            };
                            var acc=JsonSerializer.Deserialize<GoogleAccountResponse>(strData, options);
                            var cus = AccountDAO.Instance.GetAccounts().Where(a => a.Email.Equals(acc.Email)).SingleOrDefault();
                            if (cus==null)
                                throw new CrudException(HttpStatusCode.BadRequest, "User Not Found", "");
                            else
                            {
                                var user = _mapper.Map<Account, AccountResponse>(cus);
                                var a = GenerateJwtToken(cus);
                                user.Token = a.AccessToken;
                                user.RefreshToken = a.RefreshToken;
                                return user;
                            }
                        }
                    }
                }
            }
            return null;

        }
        public async Task<AccountResponse> GetAccountById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Account Invalid", "");
                }
                var response = AccountDAO.Instance.GetAccounts().Where(s => s.Id == id).SingleOrDefault();

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found account with id {id.ToString()}", "");
                }

                return _mapper.Map<AccountResponse>(response);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Account By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<AccountResponse> GetToUpdateStatus(int id)
        {
            try
            {
                Account customer = null;
                customer = AccountDAO.Instance.GetAccounts().Where(s => s.Id == id).SingleOrDefault();

                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found account with id{id.ToString()}", "");
                }
                customer.Status = false;
                AccountDAO.Instance.Update(customer, id);
                return _mapper.Map<Account, AccountResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update account error!!!!!", ex.Message);
            }
        }

        public async Task<AccountResponse> Login(LoginRequest request)
        {
            try
            {
                Account user = new Account();
                string pass = _config["AdminAccount:Password"];
                if (request.Email.Equals(_config["AdminAccount:Email"]) && request.Password.Equals(pass))
                    user.Name = "Admin";
                else
                {
                    user = AccountDAO.Instance.GetAccounts()
                   .FirstOrDefault(u => u.Email.Equals(request.Email.Trim()) && u.RoleId==2);

                    if (user == null) throw new CrudException(HttpStatusCode.BadRequest, "User Not Found", "");
                    if (!VerifyPasswordHash(request.Password.Trim(), user.PasswordHash, user.PasswordSalt))
                        throw new CrudException(HttpStatusCode.BadRequest, "Password is incorrect", "");
                    if (user.Status == false) throw new CrudException(HttpStatusCode.BadRequest, "Your account is block", "");
                }
                var cus = _mapper.Map<Account, AccountResponse>(user);
                var acc = GenerateJwtToken(user);
                cus.Token = acc.AccessToken;
                cus.RefreshToken=acc.RefreshToken;
                return cus;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }
        }
       public async Task<AccountResponse>VerifyAndGenerateToken(TokenRequest request)
        {
            try
            {              
                var jwtTokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_config["ApiSetting:Secret"]);
                TokenValidationParameters tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
                var tokenInVerification = jwtTokenHandler.ValidateToken(request.Token, tokenValidationParameters, out var tokenValidation);
                if (tokenValidation is JwtSecurityToken securityToken)
                {
                    var rs = securityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase);
                    if (rs == false) return null;
                }

                var utcExpiredDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);
                var t = tokenInVerification.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
                var expiredDate = DateTimeOffset.FromUnixTimeSeconds(utcExpiredDate).DateTime;
                if (expiredDate > DateTime.UtcNow) throw new CrudException(HttpStatusCode.BadRequest, "Token is not expried", "");

                Account acc = new Account();
                if (request.RefreshToken.Equals(_config["AdminAccount:RefreshToken"]))
                {
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    if (_config["AdminAccount:JwtId"] != jti) throw new CrudException(HttpStatusCode.BadRequest, "Invalid Token", "");
                    if (DateTime.Parse(_config["AdminAccount:ExpiredDate"]) < DateTime.UtcNow) throw new CrudException(HttpStatusCode.BadRequest, "Expired Token", "");
                    acc.Name = "Admin";
                    acc.JwtId = _config["AdminAccount:JwtId"];
                }


                else
                {
                     acc = AccountDAO.Instance.GetAccounts().Where(a => a.Token == request.RefreshToken).FirstOrDefault();
                    if (acc == null) throw new CrudException(HttpStatusCode.BadRequest, "Invalid Token", "");
                    var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                    if (acc.JwtId != jti) throw new CrudException(HttpStatusCode.BadRequest, "Invalid Token", "");
                    if (acc.ExpiredDate < DateTime.UtcNow) throw new CrudException(HttpStatusCode.BadRequest, "Expired Token", "");
                }
                var cus = _mapper.Map<Account, AccountResponse>(acc);
                    var account = GenerateJwtToken(acc);
                    cus.Token = account.AccessToken;
                    cus.RefreshToken = account.RefreshToken;
                    return cus;
                }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }
        }
        public async Task<AccountResponse> RevokeRefreshToken(string email)
        {
            try
            {
                Account customer = new Account();
                if (email.Equals(_config["AdminAccount:Email"]))
                {
                    cacheService.RemoveData($"{_config["AdminAccount:JwtId"]}");
                    customer.Name = "Admin";
                }
                else
                {
                    customer = AccountDAO.Instance.GetAccounts().Where(a => a.Email.Equals(email)).SingleOrDefault();
                    if (customer == null)
                    {
                        throw new CrudException(HttpStatusCode.NotFound, $"Not found account with gmail {email}", "");
                    }
                    cacheService.RemoveData($"{customer.JwtId}");
                    customer.Token = null;
                    customer.AddedDate = null;
                    customer.JwtId = null;
                    customer.ExpiredDate = null;
                    AccountDAO.Instance.Update(customer, customer.Id);
                }
                var cus = _mapper.Map<Account, AccountResponse>(customer);
                return cus;
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Progress Error!!!", ex.InnerException?.Message);
            }
        }
        public async Task<AccountResponse> UpdateAccount(int accountId, UpdateAccountRequest request)
        {
            try
            {
                Account customer = null;
                customer = AccountDAO.Instance.GetAccounts()
                    .FirstOrDefault(u => u.Id == accountId);

                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found customer with id{accountId.ToString()}", "");
                }                
                _mapper.Map<UpdateAccountRequest, Account>(request, customer);
                if (request.OldPassword != null && request.NewPassword != null)
                {
                    if (!VerifyPasswordHash(request.OldPassword.Trim(), customer.PasswordHash, customer.PasswordSalt))
                        throw new CrudException(HttpStatusCode.BadRequest, "Old Password is not match", "");
                    CreatPasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    customer.PasswordHash = passwordHash;
                    customer.PasswordSalt = passwordSalt;
                }
                AccountDAO.Instance.Update(customer, accountId);
                return _mapper.Map<Account, AccountResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update customer error!!!!!", ex.Message);
            }
        }
        public string GenerateRandomNo()
        {
            int _min = 0000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max).ToString();
        }
         
        public async Task<dynamic> CreateMailMessage(string email)
        {
            bool success = false;
            string token = "";
             var randomToken = GenerateRandomNo();
            string to = email;
            string from = "anhtthse161223@fpt.edu.vn";

            var acc = AccountDAO.Instance.GetAccounts().Where(a => a.Email.Equals(email) && a.Role.Name.Equals("Lecturer")).SingleOrDefault();
            if (acc == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, $"Not found account with gmail {email}", "");
            }

            MailMessage message = new MailMessage(from, to);
            message.Subject = "Account Verification Code";
            message.Body = $"<p> Hi {acc.Name}, </p> \n <span> <p> We received a request to access your Account {email} through your email address. Your Account verification code is:</p></span>\n" +
                $"<div style=\"text-align:center\"<p dir=\"ltr\"><strong style= \"text-align:center;font-size:24px;font-weight:bold\">{randomToken}</strong></p></div>";
            AlternateView htmlView = AlternateView.CreateAlternateViewFromString(message.Body, null, "text/html");
            message.AlternateViews.Add(htmlView);

            message.IsBodyHtml = true;
            SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
            SmtpServer.UseDefaultCredentials = false;

            SmtpServer.Port = 587;
            SmtpServer.Credentials = new System.Net.NetworkCredential("anhtthse161223@fpt.edu.vn", "zbegfpzcjyljlegm");
            SmtpServer.EnableSsl = true;

            try
            {
                SmtpServer.Send(message);
                success = true;
                token = randomToken;
            }
            catch (Exception ex)
            {
                success = false;
                token = null;
                throw new Exception(ex.Message);
            }
            return new
            {
                Success = success,
                Token = token
            };
        }
        private void CreatPasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
        private dynamic GenerateJwtToken(Account? customer)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config["ApiSetting:Secret"]);
            var tokenDescriptor = new SecurityTokenDescriptor();
            if (customer.Name != "Admin")
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
                 {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Role, customer.Role.Name),
                new Claim(ClaimTypes.Name , customer.Name),
                new Claim(ClaimTypes.Email , customer.Email),
               new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.MobilePhone , customer.Phone),
                 });
                customer.Token = GenerateRefreshToken();
                customer.AddedDate = DateTime.UtcNow;
                customer.ExpiredDate = DateTime.UtcNow.AddMonths(6);
            }
            else
            {
                tokenDescriptor.Subject = new ClaimsIdentity(new Claim[]
                {
                     new Claim(ClaimTypes.NameIdentifier, "0"),
                new Claim(ClaimTypes.Role, "Admin"),
                 new Claim(ClaimTypes.Name , customer.Name),
                  new Claim(JwtRegisteredClaimNames.Jti,_config["AdminAccount:JwtId"])
                });
                _config["AdminAccount:ExpiredDate"] = DateTime.UtcNow.AddMonths(6).ToString();
                customer.Token = _config["AdminAccount:RefreshToken"];
                customer.ExpiredDate = DateTime.Parse(_config["AdminAccount:ExpiredDate"]);
                customer.JwtId = _config["AdminAccount:JwtId"];
            }
            tokenDescriptor.Expires = DateTime.Now.AddMinutes(2);
            tokenDescriptor.SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var rs=new JwtSecurityTokenHandler().WriteToken(token);
            if (customer.Name != "Admin")
            {
                customer.JwtId = token.Id;
                AccountDAO.Instance.Update(customer, customer.Id);
            }

            var expiryTime = DateTimeOffset.Now.AddMinutes(2);
            cacheService.SetData<string>($"{customer.JwtId}", customer.JwtId, expiryTime);
            return new
            {
                AccessToken = rs,
                RefreshToken = customer.Token
            };
            
        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<List<AccountResponse>> GetAccounts()
        {
            try
            {
                var customers = _mapper.Map<List<AccountResponse>>(AccountDAO.Instance.GetAccounts()
                                           .ToList());
                return customers;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get account list error!!!!!", ex.Message);
            }
        }

        public async Task<AccountResponse> UpdatePass(ResetPasswordRequest request)
        {
            try
            {
                Account customer = null;
                customer = AccountDAO.Instance.GetAccounts().Where(a => a.Email.Equals(request.Email)).SingleOrDefault();
                if (customer == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found account with gmail{request.Email}", "");
                }
                    CreatPasswordHash(request.NewPassword, out byte[] passwordHash, out byte[] passwordSalt);
                    customer.PasswordHash = passwordHash;
                    customer.PasswordSalt = passwordSalt;
                    AccountDAO.Instance.Update(customer, customer.Id);

                return _mapper.Map<Account, AccountResponse>(customer);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Password account error!!!!!", ex.Message);
            }
        }

        public async Task<List<StudentInSemesterResponse>> GetAccountsInSemester()
        {
            try
            {
                var accounts = _mapper.Map<List<StudentInSemesterResponse>>(StudentInSemesterDAO.Instance.GetStudentInSemesters()
                                        .ToList());
                return accounts;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get account list error!!!!!", ex.Message);
            }
        }
    }
}
