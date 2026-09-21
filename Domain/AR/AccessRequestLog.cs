using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class AccessRequestLog
    {
        public long AccessRequestLogId { get; set; }

        [Display(Name = "Access Request")]
        public long AccessRequestId { get; set; }
        public virtual AccessRequest AccessRequest { get; set; }

        [Required]
        [Display(Name = "Status")]
        public AccessRequestStatusEnum Status { get; set; }


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
