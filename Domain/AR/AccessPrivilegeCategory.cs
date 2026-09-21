using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class AccessPrivilegeCategory
    {
        public long AccessPrivilegeCategoryId { get; set; }

        [Required]
        [Display(Name = "Privilege Category")]
        public string Name { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        //public virtual ICollection<AccessApplication> AccessApplications { get; set; }

    }
}
