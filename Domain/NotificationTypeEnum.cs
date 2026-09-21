using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum NotificationTypeEnum : int
    {

        [Display(Name = "Routing Queue - Initiator")]
        RoutingQueueInitiator = 1,


        [Display(Name = "Route Back - Initiator")]
        RouteBackInitiator = 2,

    }
}
