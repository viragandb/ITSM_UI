using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class WorkflowLevel
    {
        public long WorkflowLevelId { get; set; }


        [Required]
        [Display(Name = "Workflow")]
        public long WorkflowId { get; set; }
        public virtual Workflow Workflow { get; set; }

        [Display(Name = "Team")]
        public long TeamId { get; set; }
        public virtual Team Team { get; set; }

        [Display(Name = "Level")]
        public int LevelNo { get; set; }

        [Display(Name = "Level Type")]
        public WorkflowLevelTypeEnum WorkflowLevelType { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
