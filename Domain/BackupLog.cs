using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
      public class BackupLog
    {
        public long BackupLogId { get; set; }

        [Display(Name = "Status")]
        public BackupJobStatusEnum Status { get; set; }

        [Display(Name = "Backup Job")]
        public long BackupJobId { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }

        public virtual BackupJob BackupJob { get; set; }
    }
}
