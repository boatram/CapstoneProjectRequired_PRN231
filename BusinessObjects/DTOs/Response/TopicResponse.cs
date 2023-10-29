using BusinessObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Response
{
    public class TopicResponse
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public string SemesterCode { get; set; } = null!;
        public int SemesterId { get; set; }
        public string SpecializationName { get; set; } = null!;
        public virtual ICollection<TopicOfLecturerResponse> TopicOfLecturers { get; set; }

        public TopicResponse(int id, string name, string? description, bool? status, string semesterCode, int semesterId, string specializationName)
        {
            Id = id;
            Name = name;
            Description = description;
            Status = status;
            SemesterCode = semesterCode;
            SemesterId = semesterId;
            SpecializationName = specializationName;
        }
    }
}
