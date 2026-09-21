using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum TicketStatusEnum:int
    {

        [Display(Name = "Open")]
        Waiting = 1,

        [Display(Name = "Pending")]
        Pending = 5,
        //********** In Progress

        [Display(Name = "Waiting")]
        WaitingFor = 10,


        [Display(Name = "Backup Device Provided")]
        BackupDeviceProvided = 11,

        [Display(Name = "In Progress")]
        InProgress = 15,

        [Display(Name = "Disk Encryption or Preparation Sheet Checking")]
        DiskEncSheetPrepCheck = 17,

        [Display(Name = "Waiting for Business line approval")]
        BusinessLineApproval = 18,



        [Display(Name = "Under Procurement")]
        UnderProcurement = 20,

        [Display(Name = "Ready to Send")]
        ReadyToSend = 22,



        //*********** Vender
        [Display(Name = "Device In Transit")]
        InTransit = 25,

        [Display(Name = "Escalated To Vendor")]
        Vendor = 30,

        [Display(Name = "Under Repair with Vendor")]
        UnderRepair = 31,
        
        [Display(Name = "Warranty Claim ")]
        WarrantyClaim = 33,

        //******** 40


        [Display(Name = "Completed")]
        Completed = 80,

        //*************************
        [Display(Name = "Rated & Closed")]
        Rated = 85

        //[Display(Name = "Closed")]
        //Closed = 20,

        //[Display(Name = "Reject ")]
        //Reject = 21

    }
}
