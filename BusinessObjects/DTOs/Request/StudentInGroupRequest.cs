using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
   public class StudentInGroupRequest
    {
        public string? Role { get; set; }
        public string? Description { get; set; }
        public int? StudentId { get; set; }
    }
}
