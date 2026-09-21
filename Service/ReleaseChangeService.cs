using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
     public class ReleaseChangeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ReleaseChange> GetAll(string includeProperties)
        {
            return _contextUow.ReleaseChangeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<ReleaseChange> GetReleaseChangeByReleaseId(long? id, string includeProperties)
        {
            return _contextUow.ReleaseChangeRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ReleaseId == id, includeProperties: includeProperties);
        }
        public ReleaseChange GetItem(long? id, string includeProperties)
        {
            return _contextUow.ReleaseChangeRepository.Get(e => e.ReleaseChangeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ReleaseChange GetItem(long? releaseId, long? changeId, string includeProperties)
        {
            return _contextUow.ReleaseChangeRepository.Get(e => e.ReleaseId == releaseId && e.ChangeId == changeId && e.IsDeleted == false, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ReleaseChange item)
        {
            try
            {
                _contextUow.ReleaseChangeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ReleaseChange item)
        {
            try
            {
                _contextUow.ReleaseChangeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ReleaseChange item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ReleaseChangeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAvailable(ReleaseChange item)
        {
            if (_contextUow.ReleaseChangeRepository.Get(e => e.ReleaseId == item.ReleaseId && e.ChangeId == item.ChangeId && e.IsDeleted == false).Any())
                return true;

            return false;
        }

        public bool HasRelationalData(ReleaseChange item)
        {
            //if (_contextUow.ReleaseChangeRepository.Get(e => e.ReleaseChangeId == item.ReleaseChangeId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }


    }
}