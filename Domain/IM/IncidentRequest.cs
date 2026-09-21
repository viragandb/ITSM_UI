using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
    public class IncidentRequest
    {
        public long IncidentRequestId { get; set; }
       
        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }


        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Level 01 Support")]
        public long Level01TeamId { get; set; }
        [ForeignKey("Level01TeamId")]
        public virtual Team Level01Team { get; set; }

        [Display(Name = "Pending Team")]
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
        public IncidentImpactEnum Impact { get; set; }

        [Display(Name = "Business Impact")]
        public String BusinessImpact { get; set; }

        [Display(Name = "Resolve SLA (Hrs.)")]
        public double Resolve { get; set; }
       
        [Display(Name = "Status")]
        public IncidentStatusEnum Status { get; set; }
       
        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public double SpentTimeStatus { get; set; }
        public DateTime SpentTimeSync { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }
       
        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        [Display(Name = "Lessons Learnt")]
        public string LessonsLearnt { get; set; }

        [Display(Name = "Corrective Action")]
        public string CorrectiveAction { get; set; }

        [Display(Name = "Preventive Action")]
        public string PreventiveAction { get; set; }

        [Display(Name = "Deadline Violated")]
        public bool IsTimeViolated { get; set; }
    
        [Display(Name = "Vendor")]
        public long? VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Vendor Reference #")]
        public string VendorRefNo { get; set; }
        
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<IncidentRequestUpdate> IncidentRequestUpdates { get; set; }
        public virtual ICollection<IncidentRequestLog> IncidentRequestLogs { get; set; }
        public virtual ICollection<IncidentRequestDoc> IncidentRequestDocs { get; set; }
        public virtual ICollection<IncidentRequestAsset> Assets { get; set; }
        public virtual ICollection<IncidentRequestTicket> Tickets { get; set; }

        [NotMapped]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
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
