using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
     public class IncidentRequestTicket
    {
        public long IncidentRequestTicketId { get; set; }

        [Required]
        public long IncidentRequestId { get; set; }
        public virtual IncidentRequest IncidentRequest { get; set; }

        [Required]
        [Display(Name = "Ticket")]
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
