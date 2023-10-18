using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class Topic
    {
        public Topic()
        {
            TopicOfGroups = new HashSet<TopicOfGroup>();
            TopicOfLecturers = new HashSet<TopicOfLecturer>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public int SemesterId { get; set; }
        public int SpecializationId { get; set; }

        public virtual Semester Semester { get; set; } = null!;
        public virtual Specialization Specialization { get; set; } = null!;
        public virtual ICollection<TopicOfGroup> TopicOfGroups { get; set; }
        public virtual ICollection<TopicOfLecturer> TopicOfLecturers { get; set; }
    }
}
