using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class TopicOfGroup
    {
        public int TopicId { get; set; }
        public int GroupProjectId { get; set; }
        public bool? Status { get; set; }

        public virtual GroupProject GroupProject { get; set; } = null!;
        public virtual Topic Topic { get; set; } = null!;
    }
}
