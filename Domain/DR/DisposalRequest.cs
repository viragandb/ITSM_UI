using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.RA;

namespace Domain.DR
{
    public class DisposalRequest
    {
        public long DisposalRequestId { get; set; }


        [Display(Name = "Title")]
        public string Title { get; set; }


        [Display(Name = "Status")]
        public DisposalStatusEnum Status { get; set; }


        [Display(Name = "Request Created Date")]
        public DateTime RequestCreatedDate { get; set; }


        [Display(Name = "Buyer Name")]
        public string BuyerName { get; set; }
   

        public DateTime UpdatedDate { get; set; }


        public string UpdatedBy { get; set; }


        public bool IsDeleted { get; set; }


        public virtual ICollection<DisposalItem> DisposalItems { get; set; }
        public virtual ICollection<DisposalRequestLog> DisposalRequestLogs { get; set; }
        public virtual ICollection<DisposalRequestUpdate> DisposalRequestUpdates { get; set; }
        public virtual ICollection<DisposalRequestDoc> DisposalRequestDocs { get; set; }


    }
}
