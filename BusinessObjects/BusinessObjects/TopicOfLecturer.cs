using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class TopicOfLecturer
    {
        public bool IsSuperLecturer { get; set; }
        public int LecturerId { get; set; }
        public int TopicId { get; set; }
        public bool? Status { get; set; }

        public virtual Account Lecturer { get; set; } = null!;
        public virtual Topic Topic { get; set; } = null!;
    }
}
