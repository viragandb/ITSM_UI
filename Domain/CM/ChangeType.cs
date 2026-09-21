using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    
     public class ChangeType
    {
        public long ChangeTypeId { get; set; }


        [Display(Name = "Change Type")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Change Area")]
        public long ChangeAreaId { get; set; }
        public virtual ChangeArea ChangeArea { get; set; }

        [Display(Name = "SLA Hrs.")]
        public double Sla { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
