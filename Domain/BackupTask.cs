using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupTask
    {
        public long BackupTaskId { get; set; }

        [Display(Name = "Name")]
        public string BackupTaskName { get; set; }

        [Required]
        [Display(Name = "Task Category")]
        public long BackupTaskCategoryId { get; set; }

        [Required]
        [Display(Name="Backup Run Date")]
        public DateTime BackupRunDate { get; set; }

        [Required]
        [Display(Name = "Backup Size")]
        public string BackupSize { get; set; }

        [Display(Name = "Backup Image")]
        public string BackupImageUrl { get; set; }

        [Display(Name = "Taken Action")]
        public string Comment { get; set; }

        [Display(Name = "Backup Run Date")]
        public DateTime FailedBackupRunDate { get; set; }

        [Display(Name = "Backup Size")]
        public string FailedBackupSize { get; set; }

        [Display(Name = "Backup Image")]
        public string FailedBackupImageUrl { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Time")]
        [NotMapped]
        public string Time { get; set; }

        [Display(Name = "Time")]
        [NotMapped]
        public string TimeFailed { get; set; }

        public virtual BackupTaskCategory BackupTaskCategory { get; set; }

        [Display(Name = "Backup Job")]
        public long BackupJobId { get; set; }
        public virtual BackupJob BackupJob { get; set; }

    }
}
