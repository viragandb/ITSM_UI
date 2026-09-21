using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.IM
{
    public class IncidentRequestDoc
    {
        public long IncidentRequestDocId { get; set; }

        [Display(Name = "Incident Request")]
        public long IncidentRequestId { get; set; }

        [Required]
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }

        [Required]
        [Display(Name = "Document Type")]
        [DefaultValue(1)]
        public DocTypeEnum DocType { get; set; }


        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "URL")]
        public string FileUrl { get; set; }

        public bool IsDeleted { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }


        [ForeignKey("UpdatedBy")]
        public virtual User UpdatedUser { get; set; }

    }
}
