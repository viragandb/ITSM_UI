using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class User
    {

        //public long UserId { get; set; }

        [Key]
        [MaxLength(20)]
        [Column(TypeName = "VARCHAR")]
        [StringLength(20)]
        [Display(Name = "LAN Id")]
        public string EmpNo { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [Display(Name = "Name")]
        public string FullName { get; set; }

        //[Display(Name = "Known Name")]
        //public string KnownName { get; set; }

        //[Required]
        //public long CompanyId { get; set; }

        //[Display(Name = "Company")]
        //public string CompanyName { get; set; }


        public string DepartmentName { get; set; }

        [Display(Name = "Designation")]
        public string DesignationName { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Contact No.")]
        public string ContactNo { get; set; }

        public DateTime LastActiveDate { get; set; }


        [Display(Name = "Branch")]
        public long? BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long? DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "User Status")]
        public UserStatusEnum UserStatus { get; set; }

        public string ProImageName { get; set; }

        //[Display(Name = "Supervisor Emp.#")]
        //public string SupervisorUserId { get; set; }

        public bool IsDeleted { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<UserGroup> UserGroups { get; set; }
        //public virtual Department Department { get; set; }

        public virtual ICollection<UserTeam> UserTeams { get; set; }

        public virtual ICollection<UserBranch> UserBranches { get; set; }

        public virtual ICollection<UserDepartment> UserDepartments { get; set; }


    }
}
