using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
     public class IncidentRequestUpdate
    {
        public long IncidentRequestUpdateId { get; set; }

        [Display(Name = "Incident Request")]
        public long IncidentRequestId { get; set; }
        public virtual IncidentRequest IncidentRequest { get; set; }


        [Required]
        [Display(Name = "Status")]
        public IncidentStatusEnum Status { get; set; }

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
