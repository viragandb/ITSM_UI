using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SystemEnvironmentService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<SystemEnvironment> GetAll(string includeProperties)
        {
            return _contextUow.SystemEnvironmentRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
      
        public IEnumerable<SystemEnvironment> GetAllByEnvType(SystemEnvironmentTypeEnum envType, string includeProperties)
        {
            return _contextUow.SystemEnvironmentRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.EnvironmentType == envType
            , includeProperties: includeProperties);

        }

       

        public SystemEnvironment GetItem(long? id, string includeProperties)
        {
            return _contextUow.SystemEnvironmentRepository.Get(e => e.SystemEnvironmentId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool ItemAvilable(SystemEnvironmentTypeEnum type,long assetTypeId,  string includeProperties)
        {
            return _contextUow.SystemEnvironmentRepository.Get(e => e.IsDeleted == false && e.EnvironmentType == type
             && e.AssetTypeId == assetTypeId , includeProperties: includeProperties).Any();
        }

        public bool Insert(SystemEnvironment item)
        {
            try
            {
                _contextUow.SystemEnvironmentRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(SystemEnvironment item)
        {
            try
            {
                _contextUow.SystemEnvironmentRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(SystemEnvironment item)
        {
            try
            {
                //item.IsDeleted = true;
                _contextUow.SystemEnvironmentRepository.Delete(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(SystemEnvironment item)
        {
            if (_contextUow.SystemAccessItemRepository.Get(e => e.SystemEnvironmentId == item.SystemEnvironmentId && e.IsDeleted == false).Any())
                return true;

            return false; 
        }

      
    }
}
