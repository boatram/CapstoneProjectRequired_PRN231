using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Repository;
using Repository.Helpers;
using System.Data;
using static Repository.Helpers.Enum;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PRN231.CPR.API.Controllers
{
    public class AccountsController : ODataController
    {
        private readonly IAccountRepository accountRepository;

        public AccountsController(IAccountRepository repository)
        {
            accountRepository = repository;
        }
        [Authorize]
        [EnableQuery]
        public async Task<ActionResult<IEnumerable<AccountResponse>>> Get()
        {
            var rs = await accountRepository.GetAccounts();
            return Ok(rs);
        }
        /// <summary>
        /// Get account by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<AccountResponse>> Get(int key)
        {
            var rs = await accountRepository.GetAccountById(key);
            return Ok(rs);
        }
        /// <summary>
        /// Update profile of account
        /// </summary>
        /// <param name="userRequest"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = "Student" + "," + "Lecturer")]
        public async Task<ActionResult<AccountResponse>> Put([FromBody] UpdateAccountRequest userRequest, int key)
        {
            var rs = await accountRepository.UpdateAccount(key, userRequest);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        [HttpGet("{cusId:int}/blocked-user")]
        public async Task<ActionResult<AccountResponse>> GetToUpdateStatus(int cusId)
        {
            var rs = await accountRepository.GetToUpdateStatus(cusId);
            return Ok(rs);
        }
        /// <summary>
        /// Send verification code by email
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verification")]
        public async Task<ActionResult<string>> Verification([FromQuery] string email)
        {
            var rs = await accountRepository.CreateMailMessage(email);
            return Ok(rs);
        }

        [HttpPost("token-verification")]
        public async Task<ActionResult<AccountResponse>> VerifyAndGenerateToken(TokenRequest request)
        {
            var rs = await accountRepository.VerifyAndGenerateToken(request);
            return Ok(rs);
        }
        [HttpPost("token-revoke")]
        public async Task<ActionResult<AccountResponse>> RevokeRefreshToken(string email)
        {
            var rs = await accountRepository.RevokeRefreshToken(email);
            return Ok(rs);
        }
        [AllowAnonymous]
        [HttpPost()]
        public async Task<ActionResult<AccountResponse>> Post(IFormFile file, ExcelChoice choice)
        {
            var rs = await accountRepository.CreateAccount(file, choice);
            return Ok(rs);
        }
        [AllowAnonymous]
        [HttpPost("authentication")]
        public async Task<ActionResult<AccountResponse>> Login([FromBody] LoginRequest model)
        {
            var rs = await accountRepository.Login(model);
            return Ok(rs);
        }
        [AllowAnonymous]
        [HttpPost("google-authentication")]
        public async Task<ActionResult<AccountResponse>> LoginGoogle([FromQuery] string googleId)
        {
            var rs = await accountRepository.LoginGoogle(googleId);
            return Ok(rs);
        }
        [AllowAnonymous]
        [HttpPost("forgotten-password")]
        public async Task<ActionResult<AccountResponse>> ResetPassword([FromQuery] ResetPasswordRequest resetPassword)
        {
            var rs = await accountRepository.UpdatePass(resetPassword);
            return Ok(rs);
        }


    }
}
