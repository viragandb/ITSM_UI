using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketUpdate
    {
        public long TicketUpdateId { get; set; }

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Display(Name = "Team")]
        public long TeamId { get; set; }
        public virtual Team Team { get; set; }

        [Display(Name = "Responsible Party")]
        public AllocatedTypeEnum AllocatedType { get; set; }

        [Required]
        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }

        [Display(Name = "Spent Time Hrs.")]
        public double SpentTime { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
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
