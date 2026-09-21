using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class CategoryService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Category> GetAll(string includeProperties)
        {
            return _contextUow.CategoryRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<Category> GetAllByTicketType(TicketTypeEnum ticketType, string includeProperties)
        {
            return _contextUow.CategoryRepository.GetAsNoTracking(e => e.IsDeleted == false 
            && e.RequestTypes.Any(r=> r.TicketType==ticketType && r.IsDeleted==false), includeProperties: includeProperties);
        }

        public Category GetItem(long? id, string includeProperties)
        {
            return _contextUow.CategoryRepository.Get(e => e.CategoryId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Category GetItemByName(string cat, string includeProperties)
        {
            return _contextUow.CategoryRepository.GetAsNoTracking(e => e.CategoryName.Contains(cat), includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(Category item)
        {
            try
            {
                _contextUow.CategoryRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Category item)
        {
            try
            {
                _contextUow.CategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Category item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.CategoryRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Category item)
        {
            if (_contextUow.RequestTypeRepository.Get(e => e.CategoryId == item.CategoryId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

       
    }
}

