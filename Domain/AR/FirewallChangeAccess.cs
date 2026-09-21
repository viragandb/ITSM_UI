using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class FirewallChangeAccess
    {
        public long FirewallChangeAccessId { get; set; }

        [Display(Name = "Environment Type")]
        public SystemEnvironmentTypeEnum EnvironmentType { get; set; }

        [Display(Name = "Host/Source Name")]
        public string HostSourceName { get; set; }

        [Display(Name = "Host/Source IP Address")]
        public string HostSourceIPAddress { get; set; }

        [Display(Name = "Destination Name")]
        public string DestinationName { get; set; }

        [Display(Name = "Destination IP Address")]
        public string DestinationIPAddress { get; set; }

        [Display(Name = "Firewall Service")]
        public long FirewallServiceId { get; set; }
        public virtual FirewallService FirewallService { get; set; }

        [Display(Name = "Service Port")]
        public string ServicePort { get; set; }

        [Display(Name = "Protocol Type")]
        public ProtocolTypeEnum ProtocolType { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
