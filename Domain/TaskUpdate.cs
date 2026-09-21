using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TaskUpdate
    {
        public long TaskUpdateId { get; set; }

        [Required]
        [Display(Name = "Task")]
        public long ScheduledTaskId { get; set; }

        [Required]
        [Display(Name = "Date")]
        public DateTime TaskDate { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }


        [Display(Name = "Reviewer Comment")]
        public string ReviewerComment { get; set; }

        [Display(Name = "Completed By")]
        public string CompletedBy { get; set; }

        [Display(Name = "Completed Date")]
        public DateTime CompletedDate { get; set; }

        [Display(Name = "Reviewed Date")]
        public DateTime ReviewedDate { get; set; }

        [Display(Name = "Score")]
        public double Score { get; set; }

        [Display(Name = "Allocated Score")]
        public double AllocatedScore { get; set; }

        [Display(Name = "Delayed")]
        public bool IsDelayed { get; set; }

        [Display(Name = "Follow Up Needed")]
        public bool IsFollowupNeeded { get; set; }

        [Display(Name = "Status")]
        public ScheduledTaskStatusEnum Status { get; set; }
        
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ScheduledTask ScheduledTask { get; set; }

        [ForeignKey("CompletedBy")]
        public virtual User CompletedUser { get; set; }
    }
}
