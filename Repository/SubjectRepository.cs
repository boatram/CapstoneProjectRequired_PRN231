using AutoMapper;
using BusinessObjects;
using BusinessObjects.BusinessObjects;
using BusinessObjects.DTOs.Request;
using BusinessObjects.DTOs.Response;
using DataAccess;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using Repository.Exceptions;
using Repository.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly IMapper mapper;
        private readonly ISpecializationRepository specializationRepository;
        public SubjectRepository(IMapper mapper, ISpecializationRepository specializationRepository)
        {
            this.mapper = mapper;
            this.specializationRepository = specializationRepository;
        }

        public async Task<List<SubjectResponse>> Create(IFormFile file)
        {
            try
            {
                List<SubjectRequest> list = new List<SubjectRequest>();
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
                                list = worksheet.ReadExcelToList<SubjectRequest>();
                                foreach (SubjectRequest subject in list)
                                {
                                    var sub = mapper.Map<SubjectRequest, Subject>(subject);
                                    var s = SubjectDAO.Instance.GetSubjects().Where(s => s.Code == subject.Code).SingleOrDefault();
                                    if (s != null)
                                    {
                                        throw new CrudException(HttpStatusCode.BadRequest, "Code has already !!!", "");
                                    }
                                    var cus = SubjectDAO.Instance.GetSubjects().Where(s => s.Name == subject.Name).SingleOrDefault();
                                    if (cus != null)
                                    {
                                        throw new CrudException(HttpStatusCode.BadRequest, "Name has already !!!", "");
                                    }
                                    var spec = specializationRepository.GetSpecializations().Where(a => a.Code.Equals(subject.SpecializationCode)).SingleOrDefault();
                                    if (spec==null)
                                        throw new CrudException(HttpStatusCode.NotFound, "Specialiazation not found !!!", "");
                                    if (spec.Status == false) throw new CrudException(HttpStatusCode.BadRequest, "Specialiazation is not available !!!", "");
                                    sub.Status = true;
                                    sub.SpecializationId = spec.Id;
                                    SubjectDAO.Instance.Create(sub);


                                }
                                return SubjectDAO.Instance.GetSubjects().Select(x => new SubjectResponse
                                {
                                    Id = x.Id,
                                    Code = x.Code,
                                    IsPrerequisite = x.IsPrerequisite,
                                    Name = x.Name,
                                    Status = x.Status,
                                    Specialization = mapper.Map<SpecializationResponse>(x.Specialization),
                                    SpecializationId = x.SpecializationId

                                }).ToList();
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
                throw new CrudException(HttpStatusCode.BadRequest, "Create Subject Error!!!", ex?.Message);
            }
        }

        public Subject GetSubjectByID(int? Id) => SubjectDAO.Instance.GetSubjectByID(Id);

        public IEnumerable<SubjectResponse> GetSubjectBySpecializationId(int id) => SubjectDAO.Instance.GetSubjectBySpecializationId(id);

        public IEnumerable<SubjectResponse> GetSubjectIsPrerequisite(int id) => SubjectDAO.Instance.GetSubjectIsPrerequisite(id);

        public async Task<List<SubjectResponse>> GetSubjects()
        {
            try
            {
                var rs= (SubjectDAO.Instance.GetSubjects().ToList());
                return mapper.Map<List<SubjectResponse>>(rs);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get list subject error!!!!!", ex.Message);
            }
        }

        public void UpdateStatus(int Id) => SubjectDAO.Instance.UpdateStatus(Id);
    }
}
