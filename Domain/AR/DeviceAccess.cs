using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class DeviceAccess
    {
        public long DeviceAccessId { get; set; }

        [Display(Name = "Request Type")]
        public long AccessRequestId { get; set; }
        public virtual AccessRequest AccessRequest { get; set; }

        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<DeviceAccessItem> DeviceAccessItems { get; set; }

    }
}
