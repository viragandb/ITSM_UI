using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Group
    {
        public long GroupId { get; set; }


        [Required]
        [DisplayName("Group Name")]
        public string GroupName { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        #region Navigationl Properties

        public virtual ICollection<UserGroup> UserGroups { get; set; }

        #endregion

    }
}
