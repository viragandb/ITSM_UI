namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _20251205_03 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RemoteAgreements", "LocationType", c => c.Int(nullable: false));
            AddColumn("dbo.RemoteAgreements", "WorkLocationAddress", c => c.String());
            AddColumn("dbo.RemoteAgreements", "CurrentlyResidingAddress", c => c.String());
            AddColumn("dbo.RemoteAgreements", "LocationStatus", c => c.Int(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsFreeFromHazards", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsLightingVentilation", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsErgonomicSeating", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsLocationSecure", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsFireSafety", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsConfidentialDiscussion", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsSeperateFromFamilySpace", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsDocStoredSecurely", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsCabinetAvailable", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsUnauthorizedVisibility", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsUseBankApprovedDevice", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsUseBankApprovedVPN", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsSecureWifiNetwork", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsAntivirusProtected", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsPublicWifiUse", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsInternetConn", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsBackupPower", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsOfficeMaterialUsed", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsLockAndKeyStorage", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsShared", c => c.Boolean(nullable: false));
            AddColumn("dbo.RemoteAgreements", "IsReturnedPromptly", c => c.Boolean(nullable: false));
            DropColumn("dbo.RemoteAgreements", "PlannedSchedule");
            DropColumn("dbo.RemoteAgreements", "DaysofWeek");
            DropColumn("dbo.RemoteAgreements", "PerformanceMonitoring");
            DropColumn("dbo.RemoteAgreements", "Date");
            DropColumn("dbo.RemoteAgreements", "EmployeeName");
            DropColumn("dbo.RemoteAgreements", "EmployeeAddress");
            DropColumn("dbo.RemoteAgreements", "Duration");
            DropColumn("dbo.RemoteAgreements", "WorkLocation");
            DropColumn("dbo.RemoteAgreements", "WorkHours");
            DropColumn("dbo.RemoteAgreements", "SignedbyEmployee");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RemoteAgreements", "SignedbyEmployee", c => c.String());
            AddColumn("dbo.RemoteAgreements", "WorkHours", c => c.String());
            AddColumn("dbo.RemoteAgreements", "WorkLocation", c => c.String());
            AddColumn("dbo.RemoteAgreements", "Duration", c => c.String());
            AddColumn("dbo.RemoteAgreements", "EmployeeAddress", c => c.String());
            AddColumn("dbo.RemoteAgreements", "EmployeeName", c => c.String());
            AddColumn("dbo.RemoteAgreements", "Date", c => c.DateTime(nullable: false));
            AddColumn("dbo.RemoteAgreements", "PerformanceMonitoring", c => c.String());
            AddColumn("dbo.RemoteAgreements", "DaysofWeek", c => c.String());
            AddColumn("dbo.RemoteAgreements", "PlannedSchedule", c => c.Int(nullable: false));
            DropColumn("dbo.RemoteAgreements", "IsReturnedPromptly");
            DropColumn("dbo.RemoteAgreements", "IsShared");
            DropColumn("dbo.RemoteAgreements", "IsLockAndKeyStorage");
            DropColumn("dbo.RemoteAgreements", "IsOfficeMaterialUsed");
            DropColumn("dbo.RemoteAgreements", "IsBackupPower");
            DropColumn("dbo.RemoteAgreements", "IsInternetConn");
            DropColumn("dbo.RemoteAgreements", "IsPublicWifiUse");
            DropColumn("dbo.RemoteAgreements", "IsAntivirusProtected");
            DropColumn("dbo.RemoteAgreements", "IsSecureWifiNetwork");
            DropColumn("dbo.RemoteAgreements", "IsUseBankApprovedVPN");
            DropColumn("dbo.RemoteAgreements", "IsUseBankApprovedDevice");
            DropColumn("dbo.RemoteAgreements", "IsUnauthorizedVisibility");
            DropColumn("dbo.RemoteAgreements", "IsCabinetAvailable");
            DropColumn("dbo.RemoteAgreements", "IsDocStoredSecurely");
            DropColumn("dbo.RemoteAgreements", "IsSeperateFromFamilySpace");
            DropColumn("dbo.RemoteAgreements", "IsConfidentialDiscussion");
            DropColumn("dbo.RemoteAgreements", "IsFireSafety");
            DropColumn("dbo.RemoteAgreements", "IsLocationSecure");
            DropColumn("dbo.RemoteAgreements", "IsErgonomicSeating");
            DropColumn("dbo.RemoteAgreements", "IsLightingVentilation");
            DropColumn("dbo.RemoteAgreements", "IsFreeFromHazards");
            DropColumn("dbo.RemoteAgreements", "LocationStatus");
            DropColumn("dbo.RemoteAgreements", "CurrentlyResidingAddress");
            DropColumn("dbo.RemoteAgreements", "WorkLocationAddress");
            DropColumn("dbo.RemoteAgreements", "LocationType");
        }
    }
}
