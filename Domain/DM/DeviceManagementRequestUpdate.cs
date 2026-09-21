using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DM
{
    public class DeviceManagementRequestUpdate
    {
        public long DeviceManagementRequestUpdateId { get; set; }

        [Display(Name = "Device Management Request")]
        public long DeviceManagementRequestId { get; set; }
        public virtual DeviceManagementRequest DeviceManagementRequest { get; set; }


        [Required]
        [Display(Name = "Status")]
        public DeviceManagementStatusEnum Status { get; set; }

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
