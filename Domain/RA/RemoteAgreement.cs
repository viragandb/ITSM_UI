using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.RA
{
    public class RemoteAgreement
    {
        public int RemoteAgreementId { get; set; }

        [Display(Name = "Remote Request Access")]
        public long RemoteAccessRequestId { get; set; }
        [ForeignKey("RemoteAccessRequestId")]
        public virtual RemoteAccessRequest RemoteAccessRequest { get; set; }


        //Annexure 1

        #region location_details

        [Display(Name = "Location Type")]
        public LocationTypeEnum LocationType { get; set; }


        [Display(Name = "Currently Residing Address")]
        public string CurrentlyResidingAddress { get; set; }


        [Display(Name = "Location Status")]
        public AssignedTypeEnum LocationStatus { get; set; }


        [Display(Name = " Is the location free from excessive noise or disruptions during working hours?")]
        public bool IsNoiseFree { get; set; }


        #endregion


        #region Overseas Travel & Remote Connectivity  

        [Display(Name = " Do you intend to travel overseas while using remote connectivity to the Bank?")]
        public bool WillTravelOverseas { get; set; }

        [Display(Name = "Destination Country")]
        public String DestinationCountry { get; set; }


        [Display(Name = "Duration Of Travel")]
        public String DurationOfTravel { get; set; }


        [Display(Name = "Reason For Travel")]
        public string ReasonForTtravel { get; set; }


        [Display(Name = " Confirmation that connectivity will comply with Bank security policies, including VPN \r\nusage and restrictions on public Wi-Fi.")]
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


        [Display(Name = "Smoke detectors/fire safety measures are available at the location.")]
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


        [Display(Name = "The employee will use Bank-approved devices for all official work.")]
        public bool IsUseBankApprovedDevice { get; set; }


        [Display(Name = "The employee will use Bank-approved VPN for all connections.")]
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


        [Display(Name = "Lock and key storage is available for all Bank materials when not in use.")]
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


        [Display(Name = "I agree to allow preserve the confidentiality of the corporate information during remote work ")]
        public bool Preserveconfidentiality { get; set; }

        #endregion

        


        // annexure two fields
        [Display(Name = "Signed by Employer")]
        public string SignedByEmployerId { get; set; }
        public virtual User SignedByEmployer { get; set; }

        [Display(Name = "Date of Signing")]
        public DateTime DateOfSigning { get; set; } // capture date on which the hr review it and use it fr sign


        public long? DaysOfWeek { get; set; }


        public string NoticePeriod { get; set; }


        public DateTime CreatedDate { get; set; }   // date on w hich the request was submmitted andalso shown in annexure two fr employee sign
        public string UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
