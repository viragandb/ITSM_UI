using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class BackupReviewLogVM
    {
        public long BackupReviewLogId { get; set; }

        [Required]
        [Display(Name = "Backup Date")]
        public DateTime BackupDate { get; set; }

        [Display(Name = "Type")]
        public ChecklistTypeEnum ChacklistType { get; set; }

        [Required]
        [Display(Name = "Checklist")]
        public long BackupChecklistId { get; set; }

        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Review")]
        public string ReviewId { get; set; }

        [Display(Name = "No Of Failures")]
        public double NoOfFailures { get; set; }
        public virtual ICollection<BackupChecklistItem> BackupChecklistItems { get; set; }
    }
}