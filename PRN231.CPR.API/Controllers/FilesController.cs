
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Repository;

namespace PRN231.CPR.API.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FilesController : Controller
    {
        private readonly IFileStorageService _fileStorageService;
        public const long MAX_UPLOAD_FILE_SIZE = 25000000;//File size must lower than 25MB
        public FilesController(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<string>> UploadFile(IFormFile file)
        {
            if (file.Length > MAX_UPLOAD_FILE_SIZE)
                return BadRequest("Exceed 25MB");
            string url = await _fileStorageService.UploadFileToDefaultAsync(file.OpenReadStream(), file.FileName);
            return Ok(url);
        }

    }
}
