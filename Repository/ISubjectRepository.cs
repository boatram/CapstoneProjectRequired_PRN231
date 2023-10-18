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
    public interface ISubjectRepository
    {
        void UpdateStatus(int Id);
        Task<List<SubjectResponse>> Create(IFormFile file);
        Subject GetSubjectByID(int? Id);
        IEnumerable<SubjectResponse> GetSubjectBySpecializationId(int id);
        IEnumerable<SubjectResponse> GetSubjectIsPrerequisite(int id);
        Task<List<SubjectResponse>> GetSubjects();
    }
}
