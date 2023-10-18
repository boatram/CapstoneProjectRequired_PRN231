using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SpecializationRepository : ISpecializationRepository
    {

        public void Create(SpecializationRequest s) => SpecializationDAO.Instance.Create(s);

        public Specialization GetSpecializationByID(int? ID) => SpecializationDAO.Instance.GetSpecializationByID(ID);

        public void UpdateStatus(int id) => SpecializationDAO.Instance.UpdateStatus(id);

        IEnumerable<SpecializationResponse> ISpecializationRepository.GetSpecializations() => SpecializationDAO.Instance.GetSpecializations();
    }
}
