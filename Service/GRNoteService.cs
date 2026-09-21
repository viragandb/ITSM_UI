using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class GRNoteService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<GRNote> GetAll(string includeProperties)
        {
            return _contextUow.GRNoteRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<GRNote> GetGRNs(long? vendorId, DateTime startDate, DateTime endDate
            , string gRNNo, string pONo, string invoiceNo
         , string includeProperties)
        {
            return _contextUow.GRNoteRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (gRNNo == "" || e.GRNNumber.Contains(gRNNo))
            && (pONo == "" || e.PONumber.Contains(pONo))
            && (invoiceNo == "" || e.InvoiceNumber.Contains(invoiceNo))
            && (e.VendorId == vendorId || vendorId == 0)
            && e.ReceivedDate >= startDate && e.ReceivedDate <= endDate

            , includeProperties: includeProperties);
        }


        public GRNote GetItem(long? id, string includeProperties)
        {
            return _contextUow.GRNoteRepository.Get(e => e.GRNoteId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public GRNote GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.GRNoteRepository.GetAsNoTracking(e => e.GRNoteId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(GRNote item)
        {
            try
            {
                _contextUow.GRNoteRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(GRNote item)
        {
            try
            {
                _contextUow.GRNoteRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(GRNote item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.GRNoteRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(GRNote item)
        {
            //if (_contextUow.RequestTypeRepository.Get(e => e.GRNoteId == item.GRNoteId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

    }
}
