using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Department
    {
        public long DepartmentId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Escalation Email")]
        public string EscalationEmail { get; set; }

        [Display(Name = "DAO")]
        public string DAOCode { get; set; }

        [Display(Name = "Remote Access Tolerance Rate")]
        public long ToleranceRate { get; set; } 

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<UserDepartment> UserDepartments { get; set; }

        public virtual ICollection<BranchDepartment> BranchDepartments { get; set; }

    }
}
