using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RegionService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();
        public IEnumerable<Region> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<Region> GetAll(string includeProperties)
        {
            return _contextUow.RegionRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public Region GetItem(long? id, string includeProperties)
        {
            return _contextUow.RegionRepository.Get(e => e.RegionId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public Region GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.RegionRepository.GetAsNoTracking(e => e.RegionId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Region item)
        {
            try
            {
                _contextUow.RegionRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Region item)
        {
            try
            {
                _contextUow.RegionRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Region item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.RegionRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Region item)
        {
            if (_contextUow.BranchRepository.Get(e => e.RegionId == item.RegionId && e.IsDeleted == false).Any())
                return true;

            return false;
        }

    }
}
