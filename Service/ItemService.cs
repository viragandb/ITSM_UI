using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ItemService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Item> GetAll(string includeProperties)
        {
            return _contextUow.ItemRepository.Get( includeProperties: includeProperties);
        }

        public Item GetItem(long? id, string includeProperties)
        {
            return _contextUow.ItemRepository.Get(e => e.ItemId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<Item> GetItemsByLocation(long LocationId,string includeProperties)
        {
            return _contextUow.ItemRepository.GetAsNoTracking(e => e.LocationId== LocationId, includeProperties: includeProperties);
        }

        public IEnumerable<Item> GetAvailbleItemsByLocation(long LocationId, string includeProperties)
        {
            return _contextUow.ItemRepository.GetAsNoTracking(e => e.LocationId == LocationId, includeProperties: includeProperties);
        }

        public IEnumerable<Item> SearchItem(string term, string includeProperties)
        {
            return _contextUow.ItemRepository.GetAsNoTracking(e => e.ItemName.Contains(term) || e.ItemSerialNo.Contains(term) || e.ItemModelNo.Contains(term), includeProperties: includeProperties);
        }
        public Item GetItemsByItemId(long ItemId, string includeProperties)
        {
            return _contextUow.ItemRepository.GetAsNoTracking(e => e.ItemId == ItemId, includeProperties: includeProperties).FirstOrDefault();
        }
        public Item GetItemsByItemBarCode(string BarCode, string includeProperties)
        {
            return _contextUow.ItemRepository.GetAsNoTracking(e => e.ItemBarCode == BarCode, includeProperties: includeProperties).FirstOrDefault();
        }
        public Item GetItemsBySericalNo(long ItemId, string includeProperties)
        {
            return _contextUow.ItemRepository.GetAsNoTracking(e => e.ItemId == ItemId, includeProperties: includeProperties).FirstOrDefault();
        }





        public bool Insert(Item item)
        {
            try
            {
                _contextUow.ItemRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Item item)
        {
            try
            {
                _contextUow.ItemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Item item)
        {
            try
            {
                //item.IsDeleted = true;
                _contextUow.ItemRepository.Delete(item);
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
