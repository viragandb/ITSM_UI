using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class BackupTaskCategory
    {
        public long BackupTaskCategoryId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string BackupTaskCategoryName { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
