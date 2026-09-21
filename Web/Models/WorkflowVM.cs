using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class WorkflowVM
    {
        public long WorkflowId { get; set; }

        [Required]
        [Display(Name = "Workflow")]
        public string Name { get; set; }

        [Display(Name = "Type")]
        public WorkflowTypeEnum WorkflowType { get; set; }

        public virtual List<WorkflowLevel> WorkflowLevels { get; set; }
    }
}