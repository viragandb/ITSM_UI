using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ChangeRequestTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeRequestType> GetAll(string includeProperties)
        {
            return _contextUow.ChangeRequestTypeRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
      
       
        public ChangeRequestType GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeRequestTypeRepository.Get(e => e.ChangeRequestTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ChangeRequestType item)
        {
            try
            {
                _contextUow.ChangeRequestTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeRequestType item)
        {
            try
            {
                _contextUow.ChangeRequestTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeRequestType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeRequestTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(ChangeRequestType item)
        {
            if (_contextUow.ChangeRepository.Get(e => e.ChangeRequestTypeId == item.ChangeRequestTypeId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

    }
}
