using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.CM
{
    public class ChangeRequestTask
    {
        public long ChangeRequestTaskId { get; set; }

        [Required]
        public long ChangeImplementDataId { get; set; }
        public virtual ChangeImplementData ChangeImplementData { get; set; }

        [Required]
        [Display(Name = "Change Type")]
        public long ChangeTypeId { get; set; }
        public virtual ChangeType ChangeType { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
