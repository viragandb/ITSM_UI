using Domain.CM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ChangeDowntimeVM
    {

        [Required]
        [Display(Name = "Change Implement Data")]
        public long ChangeImplementDataId { get; set; }

        public long ChangeRequestId { get; set; }
        public ChangeImplementData ChangeImplementData { get; set; }

        [Display(Name = "Downtime Duration")]
        public string Downtime { get; set; }

        [Display(Name = "Downtime Impacted Areas")]
        public string DowntimeImpactedAreas { get; set; }

        [Display(Name = "Downtime Planned Date")]
        public String DowntimePlannedDate { get; set; }

        [Display(Name = "Downtime Time Period")]
        public string DowntimeTimePeriod { get; set; }

        [Display(Name = "Downtime Purpose")]
        public string DowntimeTimePurpose { get; set; }

        public bool SendDowntimeAlert { get; set; }

        public List<DowntimeAlertEmail> DowntimeAlertEmails { get; set; }

    }
}