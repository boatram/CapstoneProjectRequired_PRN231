using BusinessObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Response
{
    public class GroupProjectResponse
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool? Status { get; set; }

        public  ICollection<StudentInGroupResponse> StudentInGroups { get; set; }
        public  ICollection<TopicOfGroupResponse>? TopicOfGroups { get; set; }
    }
}
