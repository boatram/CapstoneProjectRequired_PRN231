using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface IFileStorageService
    {
        Task<string> UploadFileToDefaultAsync(Stream fileStream, string fileName);
        Task<FileStreamResult> ExportStudentToExcel();
    }
}
