using Data;
using Domain;
using Domain.AR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.AR
{
     public class PhysicalAreaService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<PhysicalArea> GetAll(string includeProperties)
        {
            return _contextUow.PhysicalAreaRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<PhysicalArea> GetAll(long? branchId, string includeProperties)
        {
            return _contextUow.PhysicalAreaRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.BranchId == branchId
            , includeProperties: includeProperties);
        }

        public PhysicalArea GetItem(long? id, string includeProperties)
        {
            return _contextUow.PhysicalAreaRepository.Get(e => e.PhysicalAreaId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        //public IEnumerable<Branch> GetPhysicalAccessBranches(string includeProperties)
        //{
        //    return _contextUow.PhysicalAreaRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties).Select(s=>s.Branch).Distinct();
        //}


        public bool Insert(PhysicalArea item)
        {
            try
            {
                _contextUow.PhysicalAreaRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(PhysicalArea item)
        {
            try
            {
                _contextUow.PhysicalAreaRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(PhysicalArea item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.PhysicalAreaRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(PhysicalArea item)
        {
            if (_contextUow.PhysicalAccessItemRepository.Get(e => e.PhysicalAreaId == item.PhysicalAreaId && e.IsDeleted == false).Any())
                return true;

            return false;
        }


    }
}
