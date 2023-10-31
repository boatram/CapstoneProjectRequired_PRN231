using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ISemesterRepository
    {
        Task<SemesterResponse> CreateSemester(SemesterRequest request);
        Semester GetSemesterByID(int? Id);
        Task<List<StudentInSemesterResponse>> CreateStudentInSemester(IFormFile file);
        Task<List<SemesterResponse>> GetSemesters();
    }
}
