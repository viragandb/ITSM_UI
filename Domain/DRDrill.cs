using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class DRDrill
    {
        public long DRDrillId { get; set; }

        [Required]
        [Display(Name = "Schedule Date")]
        public DateTime ScheduleDate { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "DR Drill Date")]
        public DateTime DrillDate { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Reviewer Comment")]
        public string ReviewerComment { get; set; }

        [Display(Name = "No Of Incidents ")]
        public double NoOfIncidents { get; set; }


        [Display(Name = "Status")]
        public DRDrillStatusEnum Status { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
