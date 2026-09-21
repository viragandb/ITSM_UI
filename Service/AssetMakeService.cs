using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AssetMakeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<AssetMake> GetAll(string includeProperties)
        {
            return _contextUow.AssetMakeRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
       
       
        public AssetMake GetItem(long? id, string includeProperties)
        {
            return _contextUow.AssetMakeRepository.Get(e => e.AssetMakeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(AssetMake item)
        {
            try
            {
                _contextUow.AssetMakeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(AssetMake item)
        {
            try
            {
                _contextUow.AssetMakeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(AssetMake item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.AssetMakeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(AssetMake item)
        {
            //if (_contextUow.RequestTypeRepository.Get(e => e.AssetMakeId == item.AssetMakeId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }


    }
}
