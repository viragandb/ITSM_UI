using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ItemLogService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ItemLog> GetAll(string includeProperties)
        {
            return _contextUow.ItemLogRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<ItemLog> GetTicketLogs(long? itemId,string includeProperties)
        {
            return GetItemLogs(ItemTypeEnum.Ticket, itemId, includeProperties);
        }
        public IEnumerable<ItemLog> GetProblemLogs(long? itemId, string includeProperties)
        {
            return GetItemLogs(ItemTypeEnum.Problem, itemId, includeProperties);
        }
        public IEnumerable<ItemLog> GetChangeLogs(long? itemId, string includeProperties)
        {
            return GetItemLogs(ItemTypeEnum.Change, itemId, includeProperties);
        }

        public IEnumerable<ItemLog> GetReleaseLogs(long? itemId, string includeProperties)
        {
            return GetItemLogs(ItemTypeEnum.Release, itemId, includeProperties);
        }
        public IEnumerable<ItemLog> GetItemLogs(ItemTypeEnum type,long? itemId,string includeProperties)
        {
            return _contextUow.ItemLogRepository.Get(e => e.IsDeleted == false
            && e.ItemType== type && e.ItemId== itemId, includeProperties: includeProperties);
        }
        public ItemLog GetItem(long? id, string includeProperties)
        {
            return _contextUow.ItemLogRepository.Get(e => e.ItemLogId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ItemLog item)
        {
            try
            {
                _contextUow.ItemLogRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ItemLog item)
        {
            try
            {
                _contextUow.ItemLogRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ItemLog item)
        {
            try
            {
                var deleteItem = _contextUow.ItemLogRepository.GetById(item.ItemLogId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.ItemLogRepository.Delete(deleteItem);
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

        public bool HasRelationalData(ItemLog item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.ItemLogId == item.ItemLogId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        

    }
}
