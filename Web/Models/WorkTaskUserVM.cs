using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class WorkTaskUserVM
    {

        public string UserId { get; set; }
        public User TaskUser { get; set; }

        public virtual ICollection<WorkTask> Tasks { get; set; }

    }
}