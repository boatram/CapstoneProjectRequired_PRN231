using BusinessObjects.BusinessObjects;
using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Response
{
    public class SubjectResponse
    {
        public int Id { get; set; }
        public string Code { get; set; } = null!;       
        public string Name { get; set; } = null!;
        public bool? IsPrerequisite { get; set; }
        public int? SpecializationId { get; set; }
        public bool? Status { get; set; }

        public virtual SpecializationResponse? Specialization { get; set; }
    }
}
