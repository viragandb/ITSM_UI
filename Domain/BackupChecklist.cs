using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupChecklist
    {
        public long BackupChecklistId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string BackupChecklistName { get; set; }

        [Display(Name = "Type")]
        public ChecklistTypeEnum ChacklistType { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
