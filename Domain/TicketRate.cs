using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketRate
    {
        public long TicketRateId { get; set; }

        [Display(Name = "Rate")]
        public RateTypeEnum Rate { get; set; }

        [Display(Name = "Value")]
        public double RateValue { get; set; }

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Ticket Ticket { get; set; }


    }
}
