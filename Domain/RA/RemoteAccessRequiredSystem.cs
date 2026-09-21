using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public class RemoteAccessRequiredSystem
    {

        public long RemoteAccessRequiredSystemId { get; set; }


        [Display(Name = "Remote Access Request")]
        public long RemoteAccessRequestId { get; set; }
        [ForeignKey("RemoteAccessRequestId")]
        public virtual RemoteAccessRequest RemoteAccessRequest { get; set; }


        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }


        public bool IsDeleted  { get; set; }


        public DateTime UpdatedDate { get; set; }

    }
}
