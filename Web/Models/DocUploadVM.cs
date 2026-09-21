using Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DocUploadVM
    {

        [Display(Name = "Type")]
        public ItemTypeEnum ItemType { get; set; }

        [Display(Name = "Ticket")]
        public long TicketId { get; set; }
        public Ticket Ticket { get; set; }

        [Required]
        [Display(Name = "Document Name")]
        public string DocumentName { get; set; }

        [Required]
        [Display(Name = "File Name")]
        public string FileName { get; set; }

        [Display(Name = "URL")]
        public string FileUrl { get; set; }

        //public virtual ICollection<ItemDoc> ItemDocs { get; set; }
        public virtual ICollection<TicketDoc> TicketDocs { get; set; }

        public string BackController { get; set; }
        public string BackAction { get; set; }
        //public Problem Problem { get; set; }
        //public Change Change { get; set; }
        //public Release Release { get; set; }

    }
}