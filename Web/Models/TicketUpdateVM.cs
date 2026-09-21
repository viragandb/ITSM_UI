using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketUpdateVM
    {
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Pending Team")]
        public long PendingTeamId { get; set; }

        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }

        [Display(Name = "Vendor")]
        public long VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Vendor Reference #")]
        public string VendorRefNo { get; set; }

        [Display(Name = "Reason for SLA Violation")]
        public string ReasonForSLA { get; set; }

        public bool IsSLAReasonRequired { get; set; }
    }
}