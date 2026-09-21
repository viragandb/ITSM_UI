using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TicketStatusService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<TicketStatus> GetAll(string includeProperties)
        {
            return _contextUow.TicketStatusRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<TicketStatus> GetAll(long? teamId, string includeProperties)
        {
            return _contextUow.TicketStatusRepository.Get(e => e.IsDeleted == false && e.TeamId == teamId, includeProperties: includeProperties);
        }

        public IEnumerable<TicketStatus> GetAllOnlyUpdatable(long? teamId, string includeProperties)
        {
            return _contextUow.TicketStatusRepository.Get(e => e.IsDeleted == false && e.TeamId == teamId && e.AllowUpdate==true, includeProperties: includeProperties);
        }

        public bool ItemAvilable(TicketStatusEnum status, long teamId)
        {
            return _contextUow.TicketStatusRepository.Get(e => e.IsDeleted == false 
            && e.Status == status && e.TeamId == teamId
            , includeProperties: "").Any();
        }


        public TicketStatus GetItem(long? id, string includeProperties)
        {
            return _contextUow.TicketStatusRepository.Get(e => e.TicketStatusId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public TicketStatus GetItem(TicketStatusEnum status,long? teamId, string includeProperties)
        {
            return _contextUow.TicketStatusRepository.Get(e => e.IsDeleted == false && e.Status == status && e.TeamId==teamId, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool GetTimeCapture(TicketStatusEnum status,long? teamId, string includeProperties)
        {
            try
            {
                return _contextUow.TicketStatusRepository.Get(e => e.IsDeleted == false && e.Status == status && e.TeamId == teamId
                , includeProperties: includeProperties).FirstOrDefault().TimeCapture;
            }catch(Exception ex)
            {
                return false;
            }
        }
        public bool Insert(TicketStatus item)
        {
            try
            {
                _contextUow.TicketStatusRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TicketStatus item)
        {
            try
            {
                _contextUow.TicketStatusRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(TicketStatus item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TicketStatusRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(TicketStatus item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.TicketStatusId == item.TicketStatusId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public TicketStatusEnum GetDiviceInTransitStatus ()
        {
            return TicketStatusEnum.InTransit;
        }

        public TicketStatusEnum GetVendorStatus()
        {
            return TicketStatusEnum.Vendor;
        }

        public bool IsVenderSLAStatus(TicketStatusEnum status)
        {
            bool res = false;
            if(status == TicketStatusEnum.Vendor || status == TicketStatusEnum.UnderRepair || status ==TicketStatusEnum.WarrantyClaim)
                res = true;

            return res;
        }

    }
}