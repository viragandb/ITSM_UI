using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class CheklistTaskVM
    {
        public long TaskCategoryId { get; set; }

        public TaskCategory TaskCategory { get; set; }

        public virtual ICollection<TaskUpdate> TaskUpdates { get; set; }

    }
}