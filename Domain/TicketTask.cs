using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class TicketTask
    {
        public long TicketTaskId { get; set; }

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }
        public virtual Ticket Ticket { get; set; }

        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Allocated Team")]
        public long AllocatedTeamId { get; set; }
        [ForeignKey("AllocatedTeamId")]
        public virtual Team AllocatedTeam { get; set; }

        [Display(Name = "Status")]
        public TaskStatusEnum Status { get; set; }

        [Display(Name = "Spent Time")]
        public double SpentTime { get; set; }

        public bool IsMyTeam { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }
        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }

        [Display(Name = "Assigned By")]
        public string AssignedBy { get; set; }
        [ForeignKey("AssignedBy")]
        public virtual User AssignedByUser { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedToUser { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
