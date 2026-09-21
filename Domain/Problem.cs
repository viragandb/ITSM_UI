using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Problem
    {

        public long ProblemId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Priority")]
        public PriorityEnum Priority { get; set; }

        [Display(Name = "Request Type")]
        public long RequestTypeId { get; set; }

        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        [Display(Name = "Impact")]
        public string Impact { get; set; }

        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }

        [Display(Name = "Solution")]
        public string Solution { get; set; }

        [Display(Name = "Status")]
        public ProblemStatusEnum Status { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual RequestType RequestType { get; set; }

        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }

        [NotMapped]
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        [NotMapped]
        public virtual ICollection<ItemDoc> ItemsDocs { get; set; }

        [NotMapped]
        public virtual ICollection<ItemAsset> ItemAssets { get; set; }

        [NotMapped]
        public virtual ICollection<Ticket> Tickets { get; set; }
    }
}
