using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Event
    {
        public long EventId { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }


        [Display(Name = "Action Taken")]
        public string ActionTaken { get; set; }


        [Display(Name = "Event Type")]
        public long EventTypeId { get; set; }


        [Required]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }


        [Display(Name = "Status")]
        public EventStatusEnum Status { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
      
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual EventType EventType { get; set; }

        [NotMapped]
        [Display(Name = "Time")]
        public String EventTime { get; set; }


        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }

        [NotMapped]
        public virtual ICollection<ItemLog> ItemLogs { get; set; }
        [NotMapped]
        public virtual ICollection<ItemDoc> ItemsDocs { get; set; }
     

        
    }


}
