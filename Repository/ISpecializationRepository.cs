using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public interface ISpecializationRepository
    {
        void UpdateStatus(int id);
        Task<SpecializationResponse> CreateSpecialization(SpecializationRequest request);
        Specialization GetSpecializationByID(int? ID);
        IEnumerable<SpecializationResponse> GetSpecializations();
    }
}
