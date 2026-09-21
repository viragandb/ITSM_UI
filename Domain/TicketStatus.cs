using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketStatus
    {
        public long TicketStatusId { get; set; }

        [Required]
        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }

        [Display(Name = "Team")]
        public long TeamId { get; set; }

        [Display(Name = "Time Capture")]
        public bool TimeCapture { get; set; }

        [Display(Name = "Allow Update")]
        public bool AllowUpdate { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Team Team { get; set; }

        [NotMapped]
        public string StatusName { get; set; }
    }
}
