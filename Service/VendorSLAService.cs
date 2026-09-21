using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class VendorSLAService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<VendorSLA> GetAll(long? vendorId, string includeProperties)
        {
            return _contextUow.VendorSLARepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.VendorId == vendorId || vendorId == 0), includeProperties: includeProperties);
        }

        public VendorSLA GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.VendorSLARepository.GetAsNoTracking(e => e.VendorSLAId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public VendorSLA GetItem(long? id, string includeProperties)
        {
            return _contextUow.VendorSLARepository.Get(e => e.VendorSLAId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public double GetVendorSLA(long? vendorId, long? assetTypeId)
        {
            var sla = _contextUow.VendorSLARepository.Get(e => e.VendorId == vendorId && e.AssetTypeId == assetTypeId, includeProperties: "").FirstOrDefault();
            if (sla != null)
                return sla.SLA;
            else
                return 0;
        }

        public bool Insert(VendorSLA item)
        {
            try
            {
                _contextUow.VendorSLARepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(VendorSLA item)
        {
            try
            {
                _contextUow.VendorSLARepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(VendorSLA item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.VendorSLARepository.Delete(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool IsAvilable(VendorSLA item)
        {
            if (_contextUow.VendorSLARepository.Get(e => e.VendorId == item.VendorId && e.AssetTypeId == item.AssetTypeId).Any())
                return true;

            return false;
        }
        public bool HasRelationalData(VendorSLA item)
        {
            //if (_contextUow.TicketRepository.Get(e => e.VendorSLAId == item.VendorSLAId && e.IsDeleted == false).Any())
            //return true;

            return false;
        }



    }
}
