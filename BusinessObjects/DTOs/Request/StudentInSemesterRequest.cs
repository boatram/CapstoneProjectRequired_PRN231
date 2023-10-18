using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
    public class StudentInSemesterRequest
    {
        [BooleanAttribute]
        public bool Status { get; set; }
        [StringAttribute]
        public string StudentCode { get; set; }
        [StringAttribute]
        public string SubjectCode { get; set; }
        [StringAttribute]
        public string SemesterCode { get; set; }
    }
}
