using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AddChangeVM
    {
        public long ReleaseId { get; set; }
        public Release Release { get; set; }

        public virtual ICollection<Change> AssignedChanges { get; set; }
        public virtual ICollection<Change> Changes { get; set; }
    }
}