using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AddProblemVM
    {
        public long ChangeId { get; set; }
        public Change Change { get; set; }

        public virtual ICollection<Problem> AssignedProblems { get; set; }
        public virtual ICollection<Problem> Problems { get; set; }
    }
}