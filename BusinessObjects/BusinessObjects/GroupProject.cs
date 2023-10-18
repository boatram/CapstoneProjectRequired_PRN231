using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class GroupProject
    {
        public GroupProject()
        {
            StudentInGroups = new HashSet<StudentInGroup>();
            TopicOfGroups = new HashSet<TopicOfGroup>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool? Status { get; set; }

        public virtual ICollection<StudentInGroup> StudentInGroups { get; set; }
        public virtual ICollection<TopicOfGroup> TopicOfGroups { get; set; }
    }
}
