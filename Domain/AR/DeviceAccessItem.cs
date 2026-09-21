using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
     public class DeviceAccessItem
    {
        public long DeviceAccessItemId { get; set; }

        [Display(Name = "Device Access")]
        public long DeviceAccessId { get; set; }
        public virtual DeviceAccess DeviceAccess { get; set; }

        [Required]
        [Display(Name = "Device Type")]
        public long DeviceAccessItemTypeId { get; set; }
        public virtual DeviceAccessItemType DeviceAccessItemType { get; set; }

        [Display(Name = "End User Acceptance")]
        public bool EndUserAcceptance  { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
