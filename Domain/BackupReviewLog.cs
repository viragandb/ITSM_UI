using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupReviewLog
    {

        public long BackupReviewLogId { get; set; }

        [Required]
        [Display(Name = "Backup Review Date")]
        public DateTime BackupDate { get; set; }

        [Display(Name = "Type")]
        public ChecklistTypeEnum ChacklistType { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<BackupChecklistItem> BackupChecklistItems { get; set; }

        [NotMapped]
        public virtual BackupChecklistItem BackupChecklistItem { get; set; }




    }
}
