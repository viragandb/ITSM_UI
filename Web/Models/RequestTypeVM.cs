using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class RequestTypeVM
    {
        public long RequestTypeId { get; set; }

        [Display(Name = "Name")]
        public string RequestTypeName { get; set; }

        [Display(Name = "Ticket Type")]
        public TicketTypeEnum TicketType { get; set; }

        [Required]
        [Display(Name = "Team")]
        public long TeamId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }

        [Required]
        [Display(Name = "Subcategory")]
        public long SubCategoryId { get; set; }

        [Display(Name = "Asset Category")]
        public AssetCategoryEnum AssetCategory { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }

        [Display(Name = "SLA Hrs.")]
        public double Sla { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        //[Display(Name = "Request Type Priority")]
        //public long? RequestTypePriorityId { get; set; }
        //public virtual RequestTypePriority RequestTypePriority { get; set; }

        public virtual Team Team { get; set; }
        public virtual Category Category { get; set; }
        public virtual SubCategory SubCategory { get; set; }
        public virtual AssetType AssetType { get; set; }

        public List<RequestTypePriority> RequestTypePriorities { get; set; }
    }
}