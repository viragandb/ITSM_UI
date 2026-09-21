using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DR
{
    public class DisposalRequestLog
    {
        public long DisposalRequestLogId { get; set; }


        public long DisposalRequestId { get; set; }
        public virtual DisposalRequest DisposalRequest { get; set; }


        [Display(Name = "Status")]
        public DisposalStatusEnum Status { get; set; }


        [Display(Name = "Description")]
        public string Description { get; set; }


        public DateTime UpdatedDate { get; set; }


        public string UpdatedBy { get; set; }
        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }


        public bool IsDeleted { get; set; }
    }
}
