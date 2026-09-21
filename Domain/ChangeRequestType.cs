using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ChangeRequestType
    {
        public long ChangeRequestTypeId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string ChangeRequestTypeName { get; set; }

        [Display(Name = "Icon")]
        public string Icon { get; set; }

        [Display(Name = "Color")]
        public string Color { get; set; }

        [Display(Name = "Applicable for KPI")]
        public bool IsKPIApplicable { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
