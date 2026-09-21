using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DM
{
    public class DeviceManagementRequestLog
    {
        public long DeviceManagementRequestLogId { get; set; }

        [Display(Name = "Device Management Request")]
        public long DeviceManagementRequestId { get; set; }
        public virtual DeviceManagementRequest DeviceManagementRequest { get; set; }

        [Required]
        [Display(Name = "Status")]
        public DeviceManagementStatusEnum Status { get; set; }


        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }

    }
}
