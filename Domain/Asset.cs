using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Asset
    {
        public long AssetId { get; set; }


        [Display(Name = "Code")]
        public string Code { get; set; }

        [Display(Name = "GRN")]
        public long GRNoteId { get; set; }
        public virtual GRNote GRNote { get; set; }

        [Display(Name = "Status")]
        public AssetStatusEnum Status { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }

        [Required]
        [Display(Name = "Make")]
        public long AssetMakeId { get; set; }
        public virtual AssetMake AssetMake { get; set; }

        [Display(Name = "Model")]
        public string ModelName { get; set; }

        [Display(Name = "Name")]
        public string AssetName { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Fix Asset #")]
        public string AssetNo { get; set; }

        [Display(Name = "Barcode")]
        public string Barcode { get; set; }

        [Display(Name = "Serial #")]
        public string SerialNo { get; set; }

        [Required]
        [Display(Name = "Vendor")]
        public long VendorId { get; set; }
        public virtual Vendor Vendor { get; set; }

        [Display(Name = "Location")]
        public long BranchId { get; set; }
        public virtual Branch Branch { get; set; }

        [Display(Name = "Department")]
        public long DepartmentId { get; set; }
        public virtual Department Department { get; set; }

        [Display(Name = "Purchased Price")]
        public double PurchasePrice { get; set; }

        [Display(Name = "Stock In Date")]
        public DateTime StockInDate { get; set; }

        [Display(Name = "Warranty Period")]
        public double WarrantyPeriod { get; set; }

        [Display(Name = "Warranty Expire ")]
        public DateTime WarrantyExpire { get; set; }

        [Display(Name = "Maintenance Frequency")]
        public MaintenanceFrequencyEnum MaintenanceFrequency { get; set; }

        [Display(Name = "Next Maintenance Date")]
        public DateTime NextMaintenanceDate { get; set; }

        [Display(Name = "Critical")]
        public bool IsCritical { get; set; }

        [Display(Name = "Ownership")]
        public AllocatedTypeEnum AllocatedType { get; set; }

        [Display(Name = "Assigned Type")]
        public AssignedTypeEnum AssignedType { get; set; }

        [Display(Name = "To Be Returned Date")]
        public DateTime ToBeReturnedDate { get; set; }

        [Required]
        [Display(Name = "Classification")]
        public PriorityEnum Priority { get; set; }

        [Display(Name = "Device Management Type")]
        public DeviceManagementTypeEnum DeviceManagementType { get; set; }

        [Display(Name = "Responsible Team")]
        public long ResponsibleTeamId { get; set; }
        [ForeignKey("ResponsibleTeamId")]
        public virtual Team ResponsibleTeam { get; set; }

        [Display(Name = "Assigned To")]
        public string AssignedTo { get; set; }
        [ForeignKey("AssignedTo")]
        public virtual User AssignedToUser { get; set; }
        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        [Display(Name = "Last Verified By")]
        public string VerifiedBy { get; set; }

        [Display(Name = "Last Verified Date")]
        public DateTime VerifiedDate { get; set; }

        [Display(Name = "Asset Verification Request")]
        public long? AssetVerificationRequestId { get; set; }
        public virtual AssetVerificationRequest AssetVerificationRequest { get; set; }
        public virtual ICollection<AssetLog> AssetLogs { get; set; }
        public virtual ICollection<AssetTransactionLog> AssetTransactionLogs { get; set; }
        public virtual ICollection<DowntimeLog> DowntimeLogs { get; set; }
        [NotMapped]
        public virtual ICollection<TransferItem> TransferItemRequests { get; set; }

        [NotMapped] 
        public virtual ICollection<ItemAsset> Tickets { get; set; }

        public ICollection<AssetLink> ChildLinks { get; set; } = new List<AssetLink>();
        public ICollection<AssetLink> ParentLinks { get; set; } = new List<AssetLink>();


    }
}
