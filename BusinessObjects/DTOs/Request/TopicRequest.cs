using BusinessObjects.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
    public class TopicRequest
    {
        [StringAttribute]
        public string Name { get; set; } = null!;
        [StringAttribute]
        public string? Description { get; set; }
        [StringAttribute]
        public string SemesterCode { get; set; } = null!;
        [StringAttribute]
        public string SpecializationName { get; set; } = null!;
    }
}
