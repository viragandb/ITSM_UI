using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Ticket
    {
        public long TicketId { get; set; }

        [Display(Name = "Ticket Id")]
        public string Code { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Ticket Medium")]
        public TicketMediumEnum TicketMedium { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }
                      
        [Display(Name = "Department")]
        public string DepartmentName { get; set; }

        [Display(Name = "Contact Email")]
        public string ContactEmail { get; set; }

        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }


        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "Level 01 Support")]
        public long Level01TeamId { get; set; }
        [ForeignKey("Level01TeamId")]
        public virtual Team Level01Team { get; set; }

        [Display(Name = "Allocated Team")]
        public long PendingTeamId { get; set; }
        [ForeignKey("PendingTeamId")]
        public virtual Team PendingTeam { get; set; }

        [Required]
        [Display(Name = "Requested User")]
        public string RequestedBy { get; set; }
        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }

        [Display(Name = "Requested Date")]
        public DateTime RequestedDate { get; set; }

        [Display(Name = "Occurred Date")]
        public DateTime OccurredDate { get; set; }


        [Display(Name = "Impact")]
        public TicketImpactEnum Impact { get; set; }

        [Display(Name = "Urgency")]
        public TicketUrgencyEnum Urgency { get; set; }

        [Display(Name = "Priority")]
        public TicketPriorityEnum Priority { get; set; }

        [Display(Name = "Priority")]
        public long? TicketPriorityId { get; set; }
        public virtual TicketPriority TicketPriority { get; set; }

        [Display(Name = "Responsible Party")]
        public AllocatedTypeEnum AllocatedType { get; set; }

        [Display(Name = "Request Type")]
        public long RequestTypeId { get; set; }
        public virtual RequestType RequestType { get; set; }

        [Display(Name = "Respond SLA (Hrs.)")]
        public double Respond { get; set; }

        [Display(Name = "Resolve SLA (Hrs.)")]
        public double Resolve { get; set; }

        [Display(Name = "IT SLA (Hrs.)")]
        public double SLAIT { get; set; }

        [Display(Name = "Vendor SLA (Hrs.)")]
        public double SLAVendor { get; set; }

        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }

        [Display(Name = "Flow Type")]
        public TicketFlowTypeEnum FlowType { get; set; }

        [Display(Name = "Respond Time")]
        public double RespondTime { get; set; }

        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public double SpentTimeStatus { get; set; }
        public DateTime SpentTimeSync { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }

        [Display(Name = "Assigned By")]
        public string AssignedBy { get; set; }
        [ForeignKey("AssignedBy")]
        public virtual User AssignedByUser { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedToUser { get; set; }

        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        [Display(Name = "Lessons Learnt")]
        public string LessonsLearnt { get; set; }

        [Display(Name = "Corrective Action")]
        public string CorrectiveAction { get; set; }

        [Display(Name = "Preventive Action")]
        public string PreventiveAction { get; set; }


        [Display(Name = "SLA Violated")]
        public bool IsTimeViolated { get; set; }
        [Display(Name = "Rate")]
        public RateTypeEnum Rate { get; set; }

        [Display(Name = "Vendor")]
        public long? VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Vendor Reference #")]
        public string VendorRefNo { get; set; }

        [Display(Name = "Reason for SLA Violation")]
        public string ReasonForSLA { get; set; }

        [Display(Name = "Reviewed")]
        public bool IsReviewed { get; set; }

        [Display(Name = "Reopened")]
        public bool IsReopened { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<TicketUpdate> TicketUpdates { get; set; }

        public virtual ICollection<TicketLog> TicketLogs { get; set; }

        public virtual ICollection<TicketDoc> TicketDocs { get; set; }

        public virtual ICollection<TicketTask> TicketTasks { get; set; }

        public virtual ICollection<ItemAsset> ItemAssets { get; set; }

        [NotMapped]
        public virtual ICollection<Asset> Assets { get; set; }

        public string SpentTimeFomated
        {
            get
            {
                return ConvertHHmm(SpentTime);
            }
        }

        public string ConvertHHmm(double hrs)
        {
            hrs = hrs * 60;
            int n = Convert.ToInt32(hrs);
            int hour = n / 60;
            n %= 60;
            int minutes = n;
            return hour.ToString("00") + ":" + minutes.ToString("00");
        }

    }
}
