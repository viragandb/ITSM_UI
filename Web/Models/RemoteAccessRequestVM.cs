using DocumentFormat.OpenXml.Spreadsheet;
using Domain;
using Domain.AR;
using Domain.RA;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Web;

namespace Web.Models
{
    public class RemoteAccessRequestVM
    {
        public long RemoteAccessRequestId { get; set; }

        // will be hardcoded fr now

        [Display(Name = "Department ID")]
        public long SelectedDepartmentId { get; set; }


        #region Autofill fields

        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Display(Name = "LAN_ID")]
        public string LAN_ID { get; set; }


        [Display(Name = "Designation")]
        public string Designation { get; set; }


        #endregion

        #region requester details

        [Display(Name = "Location Of Requester")]
        public long BranchId { get; set; }


        [Display(Name = "Business Justification")]
        public string BusinessJustification { get; set; }


        [Display(Name = "Business Impact")]
        public string BusinessImpact { get; set; }


        [Display(Name = "Planned Schedule")]
        public PlannedSheduleEnum PlannedSchedule { get; set; }


        [Display(Name = "Remote Work Location Address")]
        public String RemoteWorkLocation { get; set; }


        [Display(Name = "Access Type ")]
        public RemoteAccessTypeEnum RemoteAccessType { get; set; }

        //reuires sys
        [Required]
        [Display(Name = "Application/System")]
        public long AssetTypeId { get; set; }
        public string AssetTypeName { get; set; }     //display


        // Dynamic list of added systems
        public List<AssetType> SelectedSystems { get; set; } = new List<AssetType>();


        [Display(Name = "Laptop/PC Number")]
        public long AssetId { get; set; }
        public string AssetName { get; set; }
 


        [Display(Name ="Start Date")]
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }


        [Display(Name = "End Date")]
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }

        public DateTime MaxEndDate { get; set; }



        [Display(Name = "Approval User")]
         public string ApprovalBy { get; set; }
        public string ApprovalByUserName { get; set; }  // display


        #endregion


        #region anexure one

        #region location_details

        [Display(Name = "Location Type")]
        public LocationTypeEnum LocationType { get; set; }


        [Display(Name = "Currently Residing Address")]
        public string CurrentlyResidingAddress { get; set; }


        [Display(Name = "Is the location a permanent or temporary arrangement?")]
        public AssignedTypeEnum LocationStatus { get; set; }


        [Display(Name = "Is the location free from excessive noise or disruptions during working hours?")]
        public bool IsNoiseFree { get; set; }

        #endregion

        #region Overseas Travel & Remote Connectivity  

        [Display(Name = "Do you intend to travel overseas while using remote connectivity to the Bank?")]
        public bool WillTravelOverseas { get; set; }

        [Display(Name = "Destination Country")]
        public String DestinationCountry { get; set; }


        [Display(Name = "Duration Of Travel")]
        public String DurationOfTravel { get; set; }


        [Display(Name = "Reason For Travel")]
        public string ReasonForTtravel { get; set; }


        [Display(Name = "Confirmation that connectivity will comply with Bank security policies, including VPN usage and restrictions on public Wi-Fi.")]
        public bool OverseasConnectivityConfirm { get; set; }

        #endregion


        #region Physical Safety & Environment

        [Display(Name = "The workspace is free from hazards (wet floors, exposed wiring, unstable furniture).")]
        public bool IsFreeFromHazards { get; set; }


        [Display(Name = "Adequate lighting and ventilation are available.")]
        public bool IsLightingVentilation { get; set; }


        [Display(Name = "Ergonomic seating and desk arrangements are in place.")]
        public bool IsErgonomicSeating { get; set; }


        [Display(Name = "The location is secure and has controlled access (doors/windows can be locked).")]
        public bool IsLocationSecure { get; set; }


        [Display(Name = " Smoke detectors / fire safety measures are available at the location.")]
        public bool IsFireSafety { get; set; }


        #endregion


        #region Confidentiality & Information Security 

        [Display(Name = "The location allows for confidential discussions without being overheard.")]
        public bool IsConfidentialDiscussion { get; set; }


        [Display(Name = "The workspace is separate from shared family spaces to ensure privacy.")]
        public bool IsSeperateFromFamilySpace { get; set; }


        [Display(Name = "Confidential documents will be securely stored when not in use.")]
        public bool IsDocStoredSecurely { get; set; }


        [Display(Name = "Lockable cabinet/drawer is available for storing physical office materials.")]
        public bool IsCabinetAvailable { get; set; }


        [Display(Name = "Screens will be positioned to avoid visibility by unauthorized persons.")]
        public bool IsUnauthorizedVisibility { get; set; }


        [Display(Name = "The employee will use Bank-approved devices for all official work")]
        public bool IsUseBankApprovedDevice { get; set; }


        [Display(Name = "The employee will use Bank-approved VPN for all connections")]
        public bool IsUseBankApprovedVPN { get; set; }


        [Display(Name = "The Wi-Fi network is secured with a strong password (WPA2 or higher encryption).")]
        public bool IsSecureWifiNetwork { get; set; }


        [Display(Name = "Antivirus and endpoint protection are enabled and updated on the device.")]
        public bool IsAntivirusProtected { get; set; }


        [Display(Name = "The employee agrees not to use public Wi-Fi without Bank-approved security measures.")]
        public bool IsPublicWifiUse { get; set; }

        #endregion


        #region Power & Internet Connectivity  

        [Display(Name = "Stable internet connectivity is available at the location.")]
        public bool IsInternetConnStable { get; set; }

        [Display(Name = "Backup power arrangements (UPS or alternative) are available to continue critical work during power failures.")]
        public bool IsBackupPower { get; set; }

        #endregion


        #region Use of Office Materials

        [Display(Name = "Office materials (documents, devices) required for remote work will be used at the location.")]
        public bool IsOfficeMaterialUsed { get; set; }


        [Display(Name = " Lock and key storage is available for all Bank materials when not in use.")]
        public bool IsLockAndKeyStorage { get; set; }


        [Display(Name = "Items will not be shared with unauthorized persons.")]
        public bool IsShared { get; set; }


        [Display(Name = "Items will be returned promptly upon request by the Bank.")]
        public bool IsReturnedPromptly { get; set; }

        #endregion


        #region General Commitments  

        [Display(Name = "I commit to adhere to all information security and confidentiality policies while working remotely.")]
        public bool AdherenceToInfoSecConfidentiality { get; set; }


        [Display(Name = "I agree to notify my supervisor if the remote location or environment changes.")]
        public bool Notifysupervisorofchanges { get; set; }


        [Display(Name = "I agree to allow preserve the confidentiality of the corporate information during remote work.")]
        public bool Preserveconfidentiality { get; set; }

        #endregion

        #endregion








    }
}