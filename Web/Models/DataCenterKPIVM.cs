using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class DataCenterKPIVM
    {
        public double KPI1Value { get; set; }
        public double KPI1ValueRatio { get; set; }
        public double KPI1WeightValue { get; set; }
        public double KPI1Weightage { get; set; }

        public virtual ICollection<Asset> Assets { get; set; }
       // public virtual ICollection<DowntimeLog> DowntimeLogs { get; set; }



        public double KPI2Value { get; set; }
        public double KPI2ValueRatio { get; set; }
        public double KPI2WeightValue { get; set; }
        public double KPI2Weightage { get; set; }
        public virtual ICollection<Change> Changes { get; set; }



        public double KPI3Value { get; set; }
        public double KPI3ValueRatio { get; set; }
        public double KPI3WeightValue { get; set; }
        public double KPI3Weightage { get; set; }
        public virtual ICollection<BackupReviewLog> BackupReviewLogs { get; set; }

        public double KPI4Value { get; set; }
        public double KPI4ValueRatio { get; set; }
        public double KPI4WeightValue { get; set; }
        public double KPI4Weightage { get; set; }
        public virtual ICollection<DRDrill> DRDrills { get; set; }

        public double KPI5Value { get; set; }
        public double KPI5ValueRatio { get; set; }
        public double KPI5WeightValue { get; set; }
        public double KPI5Weightage { get; set; }
        public virtual ICollection<DRDrill> DRDrillIIncidents { get; set; }


        public double FinalTeamKPIValue { get; set; }
        public double FinalTeamKPIWeightageValue { get; set; }
        public double FinalKPIValue { get; set; }

        public virtual ICollection<KPITaskUserVM> KPITaskUsers { get; set; }

        


    }
}