using BusinessObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs
{
    public class TopicView
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public string SemesterCode { get; set; } = null!;
        public string SpecializationName { get; set; } = null!;
        public string SuperLecturerEmail { get; set; } = null!;

        public virtual Topic? Topic { get; set; }
        public virtual Account? Lecturer { get; set; }
    }
}
