using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class VendorService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Vendor> GetAll(string includeProperties)
        {
            return _contextUow.VendorRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public Vendor GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.VendorRepository.GetAsNoTracking(e => e.VendorId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Vendor GetItem(long? id, string includeProperties)
        {
            return _contextUow.VendorRepository.Get(e => e.VendorId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Vendor item)
        {
            try
            {
                _contextUow.VendorRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Vendor item)
        {
            try
            {
                _contextUow.VendorRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Vendor item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.VendorRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Vendor item)
        {
            if (_contextUow.TicketRepository.Get(e => e.VendorId == item.VendorId && e.IsDeleted == false).Any())
                return true;

            return false;
        }



    }
}
