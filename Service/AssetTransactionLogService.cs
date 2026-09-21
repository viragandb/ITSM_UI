using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
     public class AssetTransactionLogService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AssetTransactionLog> GetAll(string includeProperties)
        {
            return _contextUow.AssetTransactionLogRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }


        public AssetTransactionLog GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetTransactionLogRepository.Get(e => e.AssetTransactionLogId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(AssetTransactionLog item)
        {
            try
            {
                _contextUow.AssetTransactionLogRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetTransactionLog item)
        {
            try
            {
                _contextUow.AssetTransactionLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AssetTransactionLog item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetTransactionLogRepository.Update(item);
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
