using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AccessControl
    {

        public long AccessControlId { get; set; }
        public long GroupId { get; set; }
        public long MenuItemFunctionId { get; set; }


        [DefaultValue("false")]
        public bool IsDeleted { get; set; }

        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual Group Group { get; set; }
        public virtual MenuItemFunction MenuItemFunction { get; set; }
    }
}

