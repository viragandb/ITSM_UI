using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class ChangeImplementData
    {
        public long ChangeImplementDataId { get; set; }

        [Required]
        [Display(Name = "Change Request")]
        public long ChangeRequestId { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }

        [Required]
        [Display(Name = "Change Impact")]
        public ChangeCategoryEnum ChangeCategory { get; set; }


        [Required]
        [Display(Name = "Priority")]
        public ChangePriorityEnum ChangePriority { get; set; }


        [Required]
        [Display(Name = "Change Area")]
        public long ChangeAreaId { get; set; }
        public virtual ChangeArea ChangeArea { get; set; }

        [Required]
        [Display(Name = "Planned Change")]
        public string PlannedChange { get; set; }

        [Required]
        [Display(Name = "Risk Identification ")]
        public string RiskIdentification { get; set; }

        [Required]
        [Display(Name = "Rollback Plan")]
        public string RollbackPlan { get; set; }

        [Display(Name = "Rollback Test Results")]
        public string RollbackTestResults { get; set; }

        [Display(Name = "Risk Assessment")]
        public string RiskAssessment { get; set; }


        [Display(Name = "Downtime Requirement")]
        public bool IsDowntimeRequired { get; set; }

        [Display(Name = "Downtime")]
        public string Downtime { get; set; }

        [Display(Name = "Downtime Impacted Areas")]
        public string DowntimeImpactedAreas { get; set; }

        [Display(Name = "Downtime Planned Date")]
        public DateTime? DowntimePlannedDate { get; set; }

        [Display(Name = "Downtime Time Period")]
        public string DowntimeTimePeriod { get; set; }

        [Display(Name = "Downtime Purpose")]
        public string DowntimeTimePurpose { get; set; }

        public bool SendDowntimeAlert { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<ChangeRequestTask> ChangeRequestTasks { get; set; }
        public virtual ICollection<DowntimeAlertEmail> DowntimeAlertEmails { get; set; }

    }
}
