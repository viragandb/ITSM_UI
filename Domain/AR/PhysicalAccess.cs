using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class PhysicalAccess
    {
        public long PhysicalAccessId { get; set; }

        public long AccessRequestId { get; set; }
        public virtual AccessRequest AccessRequest { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<PhysicalAccessItem> PhysicalAccessItems { get; set; }

    }
}
