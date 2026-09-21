using Domain;
using Domain.CM;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class NewChangeRequestVM
    {
        public long ChangeRequestId { get; set; }

        [Required]
        [Display(Name = "Change Request Category")]
        public long ChangeRequestCategoryId { get; set; }
        public virtual ChangeRequestCategory ChangeRequestCategory { get; set; }

        [Required]
        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Required]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Justification")]
        public string Justification { get; set; }

        [Display(Name = "Approval User")]
        public string ApprovalBy { get; set; }
        public string ApprovalByUserName { get; set; }

        [Display(Name = "Duration")]
        public AssignedTypeEnum DurationType { get; set; }

        [Display(Name = "Expire Date")]
        public String ExpireDate { get; set; }


    }
}