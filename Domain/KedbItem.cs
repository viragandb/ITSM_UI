using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class KedbItem
    {
        public long KedbItemId { get; set; }

    
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        [Display(Name = "Solution")]
        public string Solution { get; set; }

        [Display(Name = "Status")]
        public RequestStatusEnum Status { get; set; }

        [Display(Name = "Request Type")]
        public long RequestTypeId { get; set; }

        [Display(Name = "Ticket")]
        public long? TicketId { get; set; }

        [Display(Name = "Problem")]
        public long? ProblemId { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual RequestType RequestType { get; set; }
        public virtual Ticket Ticket { get; set; }
        public virtual Problem Problem { get; set; }

        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }




    }
}
