using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class StudentInGroup
    {
        public int Id { get; set; }
        public DateTime? JoinDate { get; set; }
        public bool? Status { get; set; }
        public string? Role { get; set; }
        public string? Description { get; set; }
        public int GroupId { get; set; }
        public int StudentId { get; set; }

        public virtual GroupProject Group { get; set; } = null!;
        public virtual Account Student { get; set; } = null!;
    }
}
