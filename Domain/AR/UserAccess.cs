using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class UserAccess
    {
        public long UserAccessId { get; set; }

        //[Required]
        [Display(Name = "Request Type")]
        public long AccessRequestId { get; set; }
        public virtual AccessRequest AccessRequest { get; set; }

        [Display(Name = "Legal Id Document")]
        public string LegalIdDoc { get; set; }

        [Display(Name = "Non-Employee Acknowledgement")]
        public string NonEmpAcknowledgementDoc { get; set; }


        [Display(Name = "Type")]
        public UserAccessTypeEnum UserAccessType { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<UserAccessItem> UserAccessItems { get; set; }

    }
}
