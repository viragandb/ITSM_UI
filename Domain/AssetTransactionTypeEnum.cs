using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum AssetTransactionTypeEnum : int
    {
        //[Display(Name = "Stock In")]
        //StockIn = 1,



        [Display(Name = "Assigned")]
        Assigned = 3,
        [Display(Name = "Transfer")]
        Transfer = 5,

        [Display(Name = "Sent To Repair")]
        SentToRepair = 6,

        [Display(Name = "To Be Disposed")]
        ToBeDisposed = 8,

        //[Display(Name = "Unassigned")]
        //Unassigned = 10,



    }
}
