using Data;
using Domain.RA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.RA
{
    public class RemoteAgreementService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<RemoteAgreement> GetAll(string includeProperties)
        {
            return _contextUow.RemoteAgreementRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public RemoteAgreement GetItem(long? id, string includeProperties)
        {
            return _contextUow.RemoteAgreementRepository.Get(e => e.RemoteAgreementId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public RemoteAgreement GetAgreementForRequest(long? RequestId)
        {
            return _contextUow.RemoteAgreementRepository.Get(e => e.RemoteAccessRequestId == RequestId).FirstOrDefault();
        }

        public RemoteAgreement GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.RemoteAgreementRepository.GetAsNoTracking(e => e.RemoteAgreementId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(RemoteAgreement item)
        {
            try
            {
                _contextUow.RemoteAgreementRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

       
        public bool Update(RemoteAgreement item)
        {
            try
            {
                _contextUow.RemoteAgreementRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(RemoteAgreement item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.RemoteAgreementRepository.Update(item);
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
