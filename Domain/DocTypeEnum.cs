using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum DocTypeEnum : int
    {
        [Display(Name = "Attachments")]
        Attachment = 1,

        //[Display(Name = "Deployment Guidelines")]
        //Deployment_Guidelines = 2,

        //[Display(Name = "Pack Release")]
        //Pack_Release = 3,
        //[Display(Name = "Vendor Solution")]
        //Vendor_Solution = 4,
        //[Display(Name = "QA Reference")]
        //QA_Reference = 5,
        
            




    }
}
