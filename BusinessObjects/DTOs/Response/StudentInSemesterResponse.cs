using BusinessObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Response
{
    public class StudentInSemesterResponse
    {
        public int Id { get; set; }
        public bool? Status { get; set; }
        public int? StudentId { get; set; }
        public int? SubjectId { get; set; }
        public int? SemesterId { get; set; }
        public string StudentCode { get; set; }
        public SubjectResponse Subject { get; set; }
        public string SemesterCode { get; set; }
    }
}
