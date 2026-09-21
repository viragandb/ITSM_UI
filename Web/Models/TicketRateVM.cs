using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketRateVM
    {
        [Display(Name = "Ticket")]
        public long TicketId { get; set; }


        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Required]
        [Display(Name = "Rate")]
        public RateTypeEnum Rate { get; set; }

        public virtual Ticket Ticket { get; set; }
    }
}