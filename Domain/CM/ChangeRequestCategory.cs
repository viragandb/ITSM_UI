using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class ChangeRequestCategory
    {
        public long ChangeRequestCategoryId { get; set; }


        [Required]
        [Display(Name = "Change Request Category")]
        public string Name { get; set; }

        public string Icon { get; set; }
        public string Color { get; set; }

        [Required]
        [Display(Name = "Team")]
        public long TeamId { get; set; }
        public virtual Team Team { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public ICollection<ChangeArea> ChangeAreas { get; set; }


    }
}
