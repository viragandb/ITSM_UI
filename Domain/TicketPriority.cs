using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketPriority
    {
        public long TicketPriorityId { get; set; }

        [Display(Name = "Name")]
        [Required]
        public string TicketPriorityName { get; set; }

        [Required]
        [Display(Name = "Priority")]
        public TicketPriorityEnum Priority { get; set; }

        [Required]
        [Display(Name = "Impact")]
        public TicketImpactEnum Impact { get; set; }

        [Required]
        [Display(Name = "Urgency")]
        public TicketUrgencyEnum Urgency { get; set; }

        //[Required]
        //[Display(Name = "Respond Hrs.")]
        //public double Respond { get; set; }

        //[Required]
        //[Display(Name = "Resolve Hrs.")]
        //public double Resolve { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
