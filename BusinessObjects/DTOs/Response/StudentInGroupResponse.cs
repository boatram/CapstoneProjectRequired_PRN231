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
    public class StudentInGroupResponse
    {
        [Key]
        public int Id { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool? Status { get; set; }
        public string? Role { get; set; }
        public string? Description { get; set; }
        [ForeignKey("GroupProjectResponse")]
        public int? GroupId { get; set; }
        public int? StudentId { get; set; }
        public string StudentCode { get; set; }
        public string? StudentName { get; set; }
        public string StudentEmail { get; set; } 
    }
}
