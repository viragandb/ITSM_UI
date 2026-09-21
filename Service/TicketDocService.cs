using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketDocService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketDoc> GetAll(string includeProperties)
        {
            return _contextUow.TicketDocRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public TicketDoc GetItem(long? id, string includeProperties)
        {
            return _contextUow.TicketDocRepository.Get(e => e.TicketDocId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public TempDoc GetTemDocAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.TempDocRepository.GetAsNoTracking(e => e.TempDocId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public IEnumerable<TicketDoc> GetTicketDocsByUserStatus(long? id, TicketStatusEnum status, string userId, string includeProperties)
        {
            return _contextUow.TicketDocRepository.Get(e => e.IsDeleted == false && e.TicketId == id
            && e.Status == status
            && e.UpdatedBy == userId
            , includeProperties: includeProperties);
        }
        public IEnumerable<TicketDoc> GetTicketDocsByUserStatus(long? id, string userId, string includeProperties)
        {
            return _contextUow.TicketDocRepository.Get(e => e.IsDeleted == false && e.TicketId == id
            && e.UpdatedBy == userId
            , includeProperties: includeProperties);
        }

        public IEnumerable<TempDoc> GetTempDocs(string userId, string includeProperties)
        {
            ItemDocService _docService = new ItemDocService();
            return _docService.GetTempDocs(userId, includeProperties);
           
        }
        public bool Insert(TicketDoc item)
        {
            try
            {
                _contextUow.TicketDocRepository.Insert(item);
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
            ItemDocService _docService = new ItemDocService();
            return _docService.InsertTempDoc(item);
           
        }
        public bool Update(TicketDoc item)
        {
            try
            {
                _contextUow.TicketDocRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketDoc item)
        {
            try
            {
                _contextUow.TicketDocRepository.Delete(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //public bool DeleteTempDoc(long? id)
        //{


        //    ItemDocService _docService = new ItemDocService();

        //    return _docService.DeleteTempDoc(id);

        //}
        public bool DeleteTempDoc(long? id,string fullPath)
        {

            TicketDocService _ticketDocService = new TicketDocService();
            var item = _ticketDocService.GetTemDocAsNoTracking(id, "");
            string _fullPath = fullPath + item.FileName;
            if (System.IO.File.Exists(_fullPath))
            {
                System.IO.File.Delete(_fullPath);
            }

            ItemDocService _docService = new ItemDocService();

            return _docService.DeleteTempDoc(id);
           
        }

        public bool HasRelationalData(TicketDoc item)
        {
            //if (_contextUow.TicketRepository.Get(e => e.TicketDocId == item.TicketDocId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public MenuItem GetBackMenuItem(TicketStatusEnum status, bool isRequestedUser)
        {
            var item = new MenuItem();
            item.Action = "Home";
            item.Controller = "Index";

            if (status==TicketStatusEnum.Pending)
            {
                item.Action = "AssignAssetDoc";
                item.Controller = "Ticket";
            }

            //if (status == TicketStatusEnum.Pending)
            //{
            //    item.Action = "Update";
            //    item.Controller = "ProcessingUnit";
            //}

            return item;
        }

    }
}
