using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TaskCategory
    {

        public long TaskCategoryId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string TaskCategoryName { get; set; }

        [Display(Name = "Weightage")]
        public double Weightage { get; set; }

        [Display(Name = "Team")]
        public long TeamId { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Team Team { get; set; }

        public virtual ICollection<ScheduledTask> ScheduledTasks { get; set; }

    }
}
