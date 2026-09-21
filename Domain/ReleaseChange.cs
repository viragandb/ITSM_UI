using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ReleaseChange
    {
        public long ReleaseChangeId { get; set; }

        [Display(Name = "Release")]
        public long ReleaseId { get; set; }


        [Display(Name = "Change")]
        public long ChangeId { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Change Change { get; set; }

    }
}
