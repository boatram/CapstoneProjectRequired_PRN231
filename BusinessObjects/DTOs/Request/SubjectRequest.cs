using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
    public class SubjectRequest
    {
        [StringAttribute]
        public string Code { get; set; } = null!;
        [StringAttribute]
        public string Name { get; set; } = null!;
        [BooleanAttribute]
        public bool? IsPrerequisite { get; set; }
        [StringAttribute]
        public string SpecializationCode { get; set; }
    }
}
