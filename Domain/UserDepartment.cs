using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
     public class UserDepartment
    {
        public long UserDepartmentId { get; set; }

        [Required]
        [Display(Name = "User")]
        public string EmpNo { get; set; }
        public virtual User User { get; set; }

        [Required]
        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [DefaultValue("false")]
        [NotMapped]
        public bool IsSelected { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

    }
}
