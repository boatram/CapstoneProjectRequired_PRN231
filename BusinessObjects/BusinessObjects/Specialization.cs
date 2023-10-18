using System;
using System.Collections.Generic;

namespace BusinessObjects.BusinessObjects
{
    public partial class Specialization
    {
        public Specialization()
        {
            Accounts = new HashSet<Account>();
            Subjects = new HashSet<Subject>();
            Topics = new HashSet<Topic>();
        }

        public int Id { get; set; }
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Subject> Subjects { get; set; }
        public virtual ICollection<Topic> Topics { get; set; }
    }
}
