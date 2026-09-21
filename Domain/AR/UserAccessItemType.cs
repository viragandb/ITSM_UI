using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class UserAccessItemType
    {
        public long UserAccessItemTypeId { get; set; }

        [Required]
        [Display(Name = "User Access Item Type")]
        public string Name { get; set; }

        [Display(Name = "Additional Info")]
        public string AdditionalInfo { get; set; }

        [Display(Name = "External Users")]
        public bool External { get; set; }

        [Display(Name = "Internal Users")]
        public bool Internal { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
