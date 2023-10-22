using BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Repository.Helpers.Enum;

namespace Repository
{
    public interface IAccountRepository
    {
        Task<List<AccountResponse>> GetAccounts();
        Task<AccountResponse> GetToUpdateStatus(int id);
        Task<dynamic> CreateMailMessage(string email);
        Task<AccountResponse> Login(LoginRequest request);
        Task<List<AccountResponse>> CreateAccount(IFormFile file, ExcelChoice choice);
        Task<List<StudentInSemesterResponse>> GetAccountsInSemester();
        Task<AccountResponse> GetAccountById(int id);
        Task<AccountResponse> LoginGoogle(string data);
        Task<AccountResponse> VerifyAndGenerateToken(TokenRequest request);
       Task<AccountResponse> UpdatePass(ResetPasswordRequest request);
        Task<AccountResponse> UpdateAccount(int accountId, UpdateAccountRequest request);
    }
}
