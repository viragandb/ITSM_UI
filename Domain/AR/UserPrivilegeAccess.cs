using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.AR
{
    public class UserPrivilegeAccess
    {
        public long UserPrivilegeAccessId { get; set; }

        [Display(Name = "Request Type")]
        public long AccessRequestId { get; set; }
        public virtual AccessRequest AccessRequest { get; set; }


        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<UserPrivilegeAccessItem> UserPrivilegeAccessItems { get; set; }
    }
}
