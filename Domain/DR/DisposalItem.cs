using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DR
{
    public class DisposalItem
    {
        public long DisposalItemId { get; set; }


        public long DisposalRequestId { get; set; }
        public virtual DisposalRequest DisposalRequest { get; set; }


        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }


        public DateTime UpdatedDate { get; set; }


        public bool IsDeleted { get; set; }
    }
}
