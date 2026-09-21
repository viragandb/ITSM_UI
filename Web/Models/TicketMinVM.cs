using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketMinVM
    {
        public long TicketId { get; set; }

        [Display(Name = "Ticket Id")]
        public string Code { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Ticket Medium")]
        public TicketMediumEnum TicketMedium { get; set; }

        [Display(Name = "Ticket Type")]
        public TicketTypeEnum TicketType { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Impact")]
        public TicketImpactEnum Impact { get; set; }

        [Display(Name = "Urgency")]
        public TicketUrgencyEnum Urgency { get; set; }

        [Display(Name = "Priority")]
        public TicketPriorityEnum Priority { get; set; }

        [Display(Name = "Priority")]
        public long TicketPriorityId { get; set; }

        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }


        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }

        [Display(Name = "Category")]
        public string CategoryName { get; set; }

        [Display(Name = "Asset Type")]
        public string AssetTypeName { get; set; }

        [Display(Name = "Subcategory")]
        public string SubCategoryName { get; set; }

        [Display(Name = "Request Type")]
        public string RequestTypeName { get; set; }

    }
}