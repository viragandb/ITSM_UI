using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupJob
    {
        public long BackupJobId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string BackupJobName { get; set; }

        [Required]
        [Display(Name = "Type")]
        public BackupTypeEnum BackupType { get; set; }

        [Display(Name = "Status")]
        public BackupJobStatusEnum Status { get; set; }

        [Display(Name = "Created User")]
        public string CreatedBy { get; set; }

        [Display(Name = "Created Date")]
        public DateTime CreatedDate { get; set; }

        [ForeignKey("CreatedBy")]
        public virtual User CreatedUser { get; set; }


        [Display(Name = "Transferred User")]
        public string TransferredBy { get; set; }
        

        [ForeignKey("TransferredBy")]
        public virtual User TransferredUser { get; set; }

        [Required]
        [Display(Name = "Backup Date")]
        public DateTime BackupDate { get; set; }

        public string BackupDateName { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        public virtual ICollection<BackupTask> BackupTasks { get; set; }
        public virtual ICollection<BackupLog> BackupLogs { get; set; }

    }
}
