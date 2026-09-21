using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
   public class ChangeRequestLog
    {
        public long ChangeRequestLogId { get; set; }

        [Display(Name = "Change Request")]
        public long ChangeRequestId { get; set; }
        public virtual ChangeRequest ChangeRequest { get; set; }

        [Required]
        [Display(Name = "Status")]
        public ChangeRequestStatusEnum Status { get; set; }


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
