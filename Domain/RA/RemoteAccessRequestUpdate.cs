using Domain.AR;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public class RemoteAccessRequestUpdate
    {
        public long RemoteAccessRequestUpdateId { get; set; }

        [Display(Name = "Remote Request Access")]
        public long RemoteAccessRequestId { get; set; }
        [ForeignKey("RemoteAccessRequestId")]
        public virtual RemoteAccessRequest RemoteAccessRequest { get; set; }

        [Display(Name = "Team")]
        public long? TeamId { get; set; }
        public virtual Team Team { get; set; }


        // team requesting clarification
        [Display(Name = "Clarification Requested By")]
        public long? ClarificationRequestedByTeamId { get; set; }

        public virtual Team ClarificationRequestedByTeam { get; set; }


        // team responding to clarification
        [Display(Name = "Clarification Assigned To")]
        public long? ClarificationAssignedToTeamId { get; set; }


        public virtual Team ClarificationAssignedToTeam { get; set; }



        [Display(Name = "Clarification Type")]
        public ClarificationType? ClarificationType { get; set; }


        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Status")]
        public AccessRequestStatusEnum Status { get; set; }

        [Display(Name = "Spent Time Hrs.")]
        public double SpentTime { get; set; }

      
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
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
