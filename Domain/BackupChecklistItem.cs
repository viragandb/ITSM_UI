using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupChecklistItem
    {
        public long BackupChecklistItemId { get; set; }


        [Required]
        [Display(Name = "Backup Review Log")]
        public long BackupReviewLogId { get; set; }

        [Required]
        [Display(Name = "Checklist")]
        public long BackupChecklistId { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "No Of Failures")]
        public double NoOfFailures { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual BackupReviewLog BackupReviewLog { get; set; }
        public virtual BackupChecklist BackupChecklist { get; set; }

    }
}
