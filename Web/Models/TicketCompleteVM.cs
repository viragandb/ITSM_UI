using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketCompleteVM
    {

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }


        //[Required]
        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        //[Required]
        [Display(Name = "Lessons Learnt")]
        public string LessonsLearnt { get; set; }

        //[Required]
        [Display(Name = "Corrective Action")]
        public string CorrectiveAction { get; set; }

        //[Required]
        [Display(Name = "Preventive Action")]
        public string PreventiveAction { get; set; }

        [Display(Name = "Reason for SLA Violation")]
        public string ReasonForSLA { get; set; }

        public bool IsSLAReasonRequired { get; set; }

    }
}