using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class DowntimeLog
    {
        public long DowntimeLogId { get; set; }

        [Required]
        [Display(Name = "Type")]
        public DowntimeTypeEnum DowntimeType { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Downtime Hrs")]
        public double DowntimeHrs { get; set; }

        public DateTime ApplicableDate { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Ticket Ticket { get; set; }
        public virtual Asset Asset { get; set; }

    }
}
