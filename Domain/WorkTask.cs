using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class WorkTask
    {
        public long WorkTaskId { get; set; }


        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Task User")]
        public string TaskUserId { get; set; }

       

        [Required]
        [Display(Name = "Task Date")]
        public DateTime TaskDate { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public DateTime EndTime { get; set; }


        [Display(Name = "Spent Hrs.")]
        public double SpentTime { get; set; }

        [Display(Name = "Type")]
        public TaskTypeEnum Type { get; set; }

        [Display(Name = "Status")]
        public TaskStatusEnum Status { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        [ForeignKey("TaskUserId")]
        public virtual User TaskUser { get; set; }
    }
}
