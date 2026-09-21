using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AssetTransactionLog
    {
        public long AssetTransactionLogId { get; set; }

        [Required]
        [Display(Name = "Asset")]
        public long AssetId { get; set; }
        public virtual Asset Asset { get; set; }

        [Required]
        [Display(Name = "Status")]
        public AssetStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public AssetTransactionTypeEnum TransactionType { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedToUser { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }
    }
}
