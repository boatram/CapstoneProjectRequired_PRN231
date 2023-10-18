using BusinessObjects.BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.DTOs.Request
{
    public class UpdateGroupProjectRequest
    {
        public string Name { get; set; } = null!;
        public virtual ICollection<StudentInGroupRequest>? StudentInGroups { get; set; }
        public int? TopicId { get; set; }
    }
}
