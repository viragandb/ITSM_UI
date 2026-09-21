using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
    public class IncidentRequestLog
    {
        public long IncidentRequestLogId { get; set; }

        [Display(Name = "Incident Request")]
        public long IncidentRequestId { get; set; }
        public virtual IncidentRequest IncidentRequest { get; set; }

        [Required]
        [Display(Name = "Status")]
        public IncidentStatusEnum Status { get; set; }


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
