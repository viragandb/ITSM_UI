using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketChangeVM
    {
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Display(Name = "Ticket Id")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }


        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Ticket Type")]
        public TicketTypeEnum TicketType { get; set; }

        public long TicketTypeId { get; set; }

        [Display(Name = "Impact")]
        public TicketImpactEnum Impact { get; set; }

        [Display(Name = "Urgency")]
        public TicketUrgencyEnum Urgency { get; set; }

        [Display(Name = "Priority")]
        public TicketPriorityEnum Priority { get; set; }

        [Display(Name = "Priority")]
        public long TicketPriorityId { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }

        [Required]
        [Display(Name = "Subcategory")]
        public long SubCategoryId { get; set; }
        public virtual SubCategory AsseSubCategorytType { get; set; }

        [Display(Name = "Request Type")]
        public long RequestTypeId { get; set; }
        public virtual RequestType RequestType { get; set; }

        public virtual TicketPriority TicketPriority { get; set; }

    }
}