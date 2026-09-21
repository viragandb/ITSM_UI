using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class AccessRequestType
    {
        public long AccessRequestTypeId { get; set; }

        [Required]
        [Display(Name = "Request Type")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public long WorkflowId { get; set; }
        public virtual Workflow Workflow { get; set; }

        [Display(Name = "IT Users Only")]
        public bool ITOnly { get; set; }

        [Display(Name = "External Users")]
        public bool External { get; set; }

        [Display(Name = "Internal Users")]
        public bool Internal { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
        public string Icon { get; set; }
        public string Color { get; set; }

        [Display(Name = "Completion Email Instructions")]
        public string CompletionEmailInstructions { get; set; }

        [Display(Name = "Requestor Instructions")]
        public string RequestorInstructions { get; set; }

        [Display(Name = "Approver Instructions")]
        public string ApproverInstructions { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
