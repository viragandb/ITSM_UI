using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ProblemVM
    {
        public long ProblemId { get; set; }


        [Display(Name = "Subject")]
        public string Subject { get; set; }

        [Display(Name = "Description")]
        public string Description { get; set; }

        [Display(Name = "Priority")]
        public PriorityEnum Priority { get; set; }

        [Display(Name = "Request Type")]
        public long RequestTypeId { get; set; }

        [Display(Name = "Root Cause")]
        public string RootCause { get; set; }

        [Display(Name = "Impact")]
        public string Impact { get; set; }

        [Display(Name = "Symptoms")]
        public string Symptoms { get; set; }

        [Display(Name = "Status")]
        public RequestStatusEnum Status { get; set; }

        [Required]
        [Display(Name = "Category")]
        public long CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [Required]
        [Display(Name = "Asset Type")]
        public long AssetTypeId { get; set; }
        public virtual AssetType AssetType { get; set; }

        [Required]
        [Display(Name = "Subcategory")]
        public long SubCategoryId { get; set; }
        public virtual SubCategory AsseSubCategorytType { get; set; }

        public string RequestedBy { get; set; }
        public DateTime RequestedDate { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }

        public virtual RequestType RequestType { get; set; }

        [ForeignKey("RequestedBy")]
        public virtual User RequestedUser { get; set; }

    }


}
