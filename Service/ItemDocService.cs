using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ItemDocService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ItemDoc> GetAll(string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<ItemDoc> GetTicketDocs(long? id,string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false && e.ItemId== id
            && e.ItemType==ItemTypeEnum.Ticket, includeProperties: includeProperties);
        }
        public IEnumerable<ItemDoc> GetProblemDocs(long? id, string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false && e.ItemId == id
            && e.ItemType == ItemTypeEnum.Problem, includeProperties: includeProperties);
        }

        public IEnumerable<ItemDoc> GetChangeDocs(long? id, string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false && e.ItemId == id
            && e.ItemType == ItemTypeEnum.Change, includeProperties: includeProperties);
        }

        public IEnumerable<ItemDoc> GetReleaseDocs(long? id, string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false && e.ItemId == id
            && e.ItemType == ItemTypeEnum.Release, includeProperties: includeProperties);
        }

        public IEnumerable<ItemDoc> GetEventDocs(long? id, string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false && e.ItemId == id
            && e.ItemType == ItemTypeEnum.Event, includeProperties: includeProperties);
        }

        public IEnumerable<ItemDoc> GetGRNDocs(long? id, string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.IsDeleted == false && e.ItemId == id
            && e.ItemType == ItemTypeEnum.GRN, includeProperties: includeProperties);
        }

        public IEnumerable<TempDoc> GetTempDocs(string userId, string includeProperties)
        {
            return _contextUow.TempDocRepository.GetAsNoTracking(e => e.IsDeleted == false && e.EmpNo == userId
            , includeProperties: includeProperties);
        }
        public ItemDoc GetItem(long? id, string includeProperties)
        {
            return _contextUow.ItemDocRepository.Get(e => e.ItemDocId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ItemDoc item)
        {
            try
            {
                _contextUow.ItemDocRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertTempDoc(TempDoc item)
        {
            try
            {
                _contextUow.TempDocRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Update(ItemDoc item)
        {
            try
            {
                _contextUow.ItemDocRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ItemDoc item)
        {
            try
            {
                #region Log
                ItemLogService _logService = new ItemLogService();
                ItemLog log = new ItemLog();
                log.ItemType = item.ItemType;
                log.ItemId = item.ItemId;
                log.Comment ="Document ("+ item.FileName+") has been Deleted";
                log.UpdatedBy = item.UpdatedBy;
                log.UpdatedDate = item.UpdatedDate;
                _logService.Insert(log);
                #endregion

                item.IsDeleted = true;
                _contextUow.ItemDocRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool DeleteTempDoc(long? id)
        {
            try
            {
                _contextUow.TempDocRepository.Delete(id);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool HasRelationalData(ItemDoc item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.ItemDocId == item.ItemDocId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

    }
}
