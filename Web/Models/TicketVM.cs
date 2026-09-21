using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketVM
    {
        public long TicketId { get; set; }

        [Display(Name = "Ticket Id")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Ticket Medium")]
        public TicketMediumEnum TicketMedium { get; set; }

        [Required]
        [Display(Name = "Ticket Type")]
        public TicketTypeEnum TicketType { get; set; }
        public long TicketTypeId { get; set; }

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



        [Display(Name = "Department")]
        public string DepartmentName { get; set; }


        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Required]
        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }
        

        [Display(Name = "Asset")]
        public long? AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        [Required]
        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }

        [Display(Name = "Occurred Date")]
        public String OccurredDate { get; set; }

        [Display(Name = "Occurred Time")]
        public String OccurredTime { get; set; }

        [Display(Name = "Respond Hrs.")]
        public double Respond { get; set; }

        [Display(Name = "Resolve Hrs.")]
        public double Resolve { get; set; }

        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        public bool IsDeleted { get; set; }
        public long UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

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
        public virtual User RequestedUser { get; set; }
        public virtual ICollection<TicketUpdate> TicketUpdates { get; set; }
        public virtual ICollection<ItemLog> ItemLogs { get; set; }

        public int PendingTickets { get; set; }
    }
}