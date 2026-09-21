using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class TicketTransferVM
    {
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Pending Team")]
        public long PendingTeamId { get; set; }
        [ForeignKey("PendingTeamId")]
        public virtual Team PendingTeam { get; set; }



    }
}