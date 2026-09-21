using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class RequestTypeService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<RequestType> GetAll(string includeProperties)
        {
            return _contextUow.RequestTypeRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<RequestType> GetAll( long? teamId,TicketTypeEnum ticketType,string includeProperties)
        {
            return _contextUow.RequestTypeRepository.Get(e => e.IsDeleted == false && e.TeamId==teamId && e.TicketType== ticketType, includeProperties: includeProperties);
        }
        public IEnumerable<RequestType> GetAll(long? teamId,long? categoryId, TicketTypeEnum ticketType, string includeProperties)
        {
            return _contextUow.RequestTypeRepository.Get(e => e.IsDeleted == false 
            && e.TeamId == teamId && e.TicketType == ticketType
            && (e.CategoryId==categoryId || categoryId==0), includeProperties: includeProperties);
        }

        //public IEnumerable<RequestType> GetAll(TicketTypeEnum ticketType, string includeProperties)
        //{
        //    return _contextUow.RequestTypeRepository.Get(e => e.IsDeleted == false && e.TicketType == ticketType 
        //    && e.CategoryId, includeProperties: includeProperties);
        //}

        public RequestType GetItem(long? id, string includeProperties)
        {
            return _contextUow.RequestTypeRepository.Get(e =>  e.RequestTypeId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool ItemAvilable(long categoryId,long assetTypeId,long subCategoryId, string includeProperties)
        {
            return _contextUow.RequestTypeRepository.Get(e => e.IsDeleted == false && e.CategoryId == categoryId
             && e.AssetTypeId == assetTypeId && e.SubCategoryId == subCategoryId, includeProperties: includeProperties).Any();
        }


        public RequestType GetItemAsNoTracking(TicketTypeEnum ticketType,long categoryId,long assetTypeId,long subCategoryId, string includeProperties)
        {
            return _contextUow.RequestTypeRepository.GetAsNoTracking(e => e.IsDeleted==false && e.TicketType == ticketType && e.CategoryId==categoryId 
            && e.AssetTypeId==assetTypeId && e.SubCategoryId==subCategoryId , includeProperties: includeProperties).FirstOrDefault();
        }

        public RequestType GetItemByNameAsNoTracking(TicketTypeEnum ticketType, string categoryName, string assetTypeName, string subCategoryName, string includeProperties)
        {
            return _contextUow.RequestTypeRepository.GetAsNoTracking(e => e.IsDeleted == false && e.TicketType == ticketType 
            && e.Category.CategoryName == categoryName
            && e.AssetType.AssetTypeName == assetTypeName
            && e.SubCategory.SubCategoryName == subCategoryName
            , includeProperties: includeProperties).FirstOrDefault();
        }
        public bool Insert(RequestType item)
        {
            try
            {
                _contextUow.RequestTypeRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(RequestType item)
        {
            try
            {
                _contextUow.RequestTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(RequestType item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.RequestTypeRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(RequestType item)
        {
            if (_contextUow.TicketRepository.Get(e => e.RequestTypeId == item.RequestTypeId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

    }
}