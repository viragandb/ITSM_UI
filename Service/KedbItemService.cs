using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class KedbItemService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<KedbItem> GetAll(string includeProperties)
        {
            return _contextUow.KedbItemRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<KedbItem> GetItems(long? teamId, long? categoryId, long? statusId, DateTime sDate, DateTime eDate, string includeProperties)
        {
            return _contextUow.KedbItemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.RequestType.TeamId == teamId || teamId == 0)
            && (e.RequestType.CategoryId == categoryId || categoryId == 0)
            && (statusId == 0 || (Int32)e.Status == statusId)
            && e.RequestedDate >= sDate && e.RequestedDate < eDate
            , includeProperties: includeProperties);
        }


        public IEnumerable<KedbItem> GetItems(long? categoryId,long? assetTypeId,long? succategoryId, string includeProperties)
        {
            return _contextUow.KedbItemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.RequestType.CategoryId == categoryId)
            && (e.RequestType.AssetTypeId == assetTypeId)
            && (e.RequestType.SubCategoryId == succategoryId)
            && (e.Status == RequestStatusEnum.Approved)
            , includeProperties: includeProperties);
        }

        public IEnumerable<KedbItem> GetByStatus(RequestStatusEnum status, string includeProperties)
        {
            return _contextUow.KedbItemRepository.Get(e => e.IsDeleted == false && e.Status==status, includeProperties: includeProperties);
        }
        public KedbItem GetItem(long? id, string includeProperties)
        {
            return _contextUow.KedbItemRepository.Get(e => e.KedbItemId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(KedbItem item)
        {
            try
            {
                _contextUow.KedbItemRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(KedbItem item)
        {
            try
            {
                _contextUow.KedbItemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(KedbItem item)
        {
            try
            {
                var deleteItem = _contextUow.KedbItemRepository.GetById(item.KedbItemId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.KedbItemRepository.Delete(deleteItem);
                    _contextUow.Save();

                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(KedbItem item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.KedbItemId == item.KedbItemId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }
    }
}
