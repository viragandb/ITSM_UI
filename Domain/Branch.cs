using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Branch
    {
        public long BranchId { get; set; }

        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Escalation Email")]
        public string EscalationEmail { get; set; }

        [Display(Name = "Region")]
        public long RegionId { get; set; }
        public virtual Region Region { get; set; }

        [Display(Name = "Delivery SLA")]
        public long DeliverySLAId { get; set; }
        public virtual DeliverySLA DeliverySLA { get; set; }

        [Display(Name = "DAO")]
        public string DAOCode { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<UserBranch> UserBranches { get; set; }


    }
}
