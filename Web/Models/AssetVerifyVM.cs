using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AssetVerifyVM
    {
        public long RequestId { get; set; }
        public virtual AssetVerificationRequest Request { get; set; }

        [Required]
        [Display(Name = "Comment")]
        public string FinalComment { get; set; }

        public List<AssetVM> Assets { get; set; }


    }
}