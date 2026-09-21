using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class SystemAccessItem
    {
        public long SystemAccessItemId { get; set; }

        //[Required]
        [Display(Name = "System Access")]
        public long SystemAccessId { get; set; }
        public virtual SystemAccess SystemAccess { get; set; }

        [Required]
        [Display(Name = "System")]
        public long SystemEnvironmentId { get; set; }
        public virtual SystemEnvironment SystemEnvironment { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Justification")]
        public string Justification { get; set; }

        [Display(Name = "Privilege Level")]
        public string PrivilegeLevel { get; set; }
        [Display(Name = "Monitored By")]
        public string MonitoredBy  { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
