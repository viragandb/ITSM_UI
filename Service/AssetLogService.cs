using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AssetLogService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AssetLog> GetAll(string includeProperties)
        {
            return _contextUow.AssetLogRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
       

        public AssetLog GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetLogRepository.Get(e => e.AssetLogId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(AssetLog item)
        {
            try
            {
                _contextUow.AssetLogRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetLog item)
        {
            try
            {
                _contextUow.AssetLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AssetLog item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       

    }
}
