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
    public class RemoteAccessRequestLog
    {
        public long RemoteAccessRequestLogId { get; set; }

        [Display(Name = "Remote Access Request")]
        public long RemoteAccessRequestId { get; set; }
        [ForeignKey("RemoteAccessRequestId")]
        public virtual RemoteAccessRequest RemoteAccessRequest { get; set; }


        [Required]
        [Display(Name = "Status")]
        public AccessRequestStatusEnum Status { get; set; }


        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
    }
}
