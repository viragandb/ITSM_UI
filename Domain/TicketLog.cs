using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketLog
    {
        public long TicketLogId { get; set; }

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Required]
        [Display(Name = "Status")]
        public TicketStatusEnum Status { get; set; }


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
