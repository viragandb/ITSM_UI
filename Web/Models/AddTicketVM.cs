using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AddTicketVM
    {
        public long ProblemId { get; set; }

        public Problem Problem { get; set; }
        public long ChangeId { get; set; }
        public Change Change { get; set; }

        public virtual ICollection<Ticket> AssignedTickets { get; set; }
        public virtual ICollection<Ticket> Tickets { get; set; }

    }
}