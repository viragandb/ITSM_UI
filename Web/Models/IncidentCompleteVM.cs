using Domain;
using Domain.IM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class IncidentCompleteVM
    {
        public long IncidentRequestId { get; set; }
        public virtual IncidentRequest IncidentRequest { get; set; }

        [Required]
        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        [Required]
        [Display(Name = "Lessons Learnt")]
        public string LessonsLearnt { get; set; }

        [Required]
        [Display(Name = "Corrective Action")]
        public string CorrectiveAction { get; set; }

        [Required]
        [Display(Name = "Preventive Action")]
        public string PreventiveAction { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public List<AssetVM> Assets { get; set; }
    }
}