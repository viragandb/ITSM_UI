using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Workflow
    {
        public long WorkflowId { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public string Name { get; set; }

        [Display(Name = "Type")]
        public WorkflowTypeEnum WorkflowType { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<WorkflowLevel> WorkflowLevels { get; set; }


    }
}
