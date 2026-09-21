using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class SerialService
    {

        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Serial> GetAll(string includeProperties)
        {
            return _contextUow.SerialRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public long GetTicketSerial()
        {
            Serial item = GetItem("TNo", "");
            Increase(item);
            return item.SerialNo;
        }
        public long GetAssetSerial()
        {
            Serial item = GetItem("ANo", "");
            Increase(item);
            return item.SerialNo;
        }

        public bool Increase(Serial item)
        {
            item.SerialNo += 1;
            return Update(item);
        }
        public bool IncreaseWith(Serial item, int no)
        {
            item.SerialNo += no;
            return Update(item);
        }
        public Serial GetItem(string code, string includeProperties)
        {
            return _contextUow.SerialRepository.Get(e => e.Code == code, includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(Serial item)
        {
            try
            {
                _contextUow.SerialRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public bool Update(Serial item)
        {
            try
            {
                _contextUow.SerialRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool Delete(Serial item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.SerialRepository.Update(item);
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
