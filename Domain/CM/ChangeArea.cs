using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class ChangeArea
    {
        public long ChangeAreaId { get; set; }

        [Display(Name = "Change Area")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Change Request Category")]
        public long ChangeRequestCategoryId { get; set; }
        public virtual ChangeRequestCategory ChangeRequestCategory { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }
  
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ICollection<ChangeType> ChangeTypes { get; set; }

    }
}
