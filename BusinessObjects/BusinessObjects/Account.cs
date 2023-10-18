using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class Account
    {
        public Account()
        {
            StudentInGroups = new HashSet<StudentInGroup>();
            StudentInSemesters = new HashSet<StudentInSemester>();
            TopicOfLecturers = new HashSet<TopicOfLecturer>();
        }

        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public string? Gender { get; set; }
        public string? Phone { get; set; }
        public int? RoleId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Avatar { get; set; }
        public bool? Status { get; set; }
        public int SpecializationId { get; set; }

        public virtual Role? Role { get; set; }
        public virtual Specialization Specialization { get; set; } = null!;
        public virtual ICollection<StudentInGroup> StudentInGroups { get; set; }
        public virtual ICollection<StudentInSemester> StudentInSemesters { get; set; }
        public virtual ICollection<TopicOfLecturer> TopicOfLecturers { get; set; }
    }
}
