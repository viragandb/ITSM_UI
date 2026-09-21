using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class UserPrivilegeAccessItem
    {
        public long UserPrivilegeAccessItemId { get; set; }

        //[Required]
        [Display(Name = "User Privilege Access")]
        public long UserPrivilegeAccessId { get; set; }
        public virtual UserPrivilegeAccess UserPrivilegeAccess { get; set; }

        [Required]
        [Display(Name = "Privilege Category")]
        public long AccessPrivilegeCategoryId { get; set; }
        public virtual AccessPrivilegeCategory AccessPrivilegeCategory { get; set; }

        [Required]
        [Display(Name = "Access Application")]
        public long AccessApplicationId { get; set; }
        public virtual AccessApplication AccessApplication { get; set; }

        [Required]
        [Display(Name = "Access Level")]
        public long AccessLevelId { get; set; }
        public virtual AccessLevel AccessLevel { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
