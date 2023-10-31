using AutoMapper;
using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Hangfire;
using Repository.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SpecializationRepository : ISpecializationRepository
    {
        private readonly IMapper _mapper;
        public SpecializationRepository(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<SpecializationResponse> CreateSpecialization(SpecializationRequest request)
        {
            try
            {
                var specialization = _mapper.Map<SpecializationRequest, Specialization>(request);
                var s = SpecializationDAO.Instance.GetSpecializations().Where(a => a.Code.Equals(request.Code)).SingleOrDefault();
                if (s != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, $"Code {request.Code} has already !!!", "");
                }
                specialization.Status = true;
                SpecializationDAO.Instance.AddNew(specialization);
                return _mapper.Map<SpecializationResponse>(specialization);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Register Error!!!", "");
            }
        }

        public Specialization GetSpecializationByID(int? ID) => SpecializationDAO.Instance.GetSpecializationByID(ID);

        public void UpdateStatus(int id) => SpecializationDAO.Instance.UpdateStatus(id);

        IEnumerable<SpecializationResponse> ISpecializationRepository.GetSpecializations() => SpecializationDAO.Instance.GetSpecializations();
    }
}
