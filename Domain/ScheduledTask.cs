using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    
    public class ScheduledTask
    {

        public long ScheduledTaskId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string TaskName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Task Category")]
        public long TaskCategoryId { get; set; }


        [Display(Name = "Repeat Type")]
        public RepeatTypeEnum RepeatType { get; set; }

     

        [Required]
        [Display(Name = "Due/Start Date")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }

        [Display(Name = "Reviewer")]
        public string ReviewBy { get; set; }

        [Display(Name = "Applicable For KPI")]
        public bool IsKPI { get; set; }

        [Display(Name = "Checklist Item")]
        public bool IsChecklist { get; set; }

        [Display(Name = "Score")]
        public double Score { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        
        [ForeignKey("AssignedTo")]
        public virtual User User { get; set; }

        [ForeignKey("ReviewBy")]
        public virtual User Reviewer { get; set; }

        public virtual TaskCategory TaskCategory { get; set; }

        public virtual ICollection<ScheduledDate> ScheduledDates { get; set; }
        public virtual ICollection<TaskUpdate> TaskUpdates { get; set; }
    }
}
