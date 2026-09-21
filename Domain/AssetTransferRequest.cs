using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class AssetTransferRequest
    {
        public long AssetTransferRequestId { get; set; }

        [Required]
        [Display(Name = "Status")]
        public AssetTransStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Transaction Type")]
        public AssetTransactionTypeEnum TransactionType { get; set; }

        [Display(Name = "Assigned Type")]
        public AssignedTypeEnum AssignedType { get; set; }


        [Display(Name = "Comment")]
        public string Comment { get; set; }

        [Display(Name = "Initiated By")]
        public string InitiatedBy { get; set; }
        [ForeignKey("InitiatedBy")]
        public virtual User InitiatedUser { get; set; }

        [Display(Name = "Initiated Date")]
        public DateTime InitiatedDate { get; set; }


        [Display(Name = "Authorized By")]
        public string AuthorizedBy { get; set; }
        [ForeignKey("AuthorizedBy")]
        public virtual User AuthorizedUser { get; set; }
        [Display(Name = "Authorizer Comment")]
        public string CommentAuthorize { get; set; }

        [Display(Name = "Authorized Date")]
        public DateTime AuthorizedDate { get; set; }


        [Display(Name = "Completed By")]
        public string CompletedBy { get; set; }

        [ForeignKey("CompletedBy")]
        public virtual User CompletedUser { get; set; }

        [Display(Name = "Final Comment")]
        public string CommentComplete { get; set; }

        [Display(Name = "Completed Date")]
        public DateTime CompletedDate { get; set; }

        [Display(Name = "Location From")]
        public long BranchIdFrom { get; set; }

        [ForeignKey("BranchIdFrom")]
        public virtual Branch BranchFrom { get; set; }

        [Display(Name = "Department From ")]
        public long DepartmentIdFrom { get; set; }

        [ForeignKey("DepartmentIdFrom")]
        public virtual Department DepartmentFrom { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedToUser { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }
        [ForeignKey("BranchId")]
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        [Display(Name = "Vendor")]
        public long? VendorId { get; set; }
        [ForeignKey("VendorId")]
        public virtual Vendor Vendor { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual ICollection<TransferItem> TransferItems { get; set; }

    }
}
