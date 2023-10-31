using BusinessObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Response
{
    public class TopicOfGroupResponse
    {
        [Key]
        public int Id { get; set; }
        public int? TopicId { get; set; }
        [Key]
        public int? GroupProjectId { get; set; }
        public bool? Status { get; set; }
        public string TopicName { get; set; }
        public string SemesterCode { get; set; }
        public string SpecializationName { get; set; }
    }
}
