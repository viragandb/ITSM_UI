using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BranchService
    {

        private readonly UnitOfWork _contextUow = new UnitOfWork();
        public IEnumerable<Branch> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<Branch> GetItemsByUser(string userId,string includeProperties)
        {
            var user = _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == userId).FirstOrDefault();

            return _contextUow.BranchRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.BranchId==user.BranchId ||  e.UserBranches.Any(u => u.EmpNo == userId)  )
            , includeProperties: includeProperties);
        }

        public IEnumerable<Branch> GetAll(string includeProperties)
        {
            return _contextUow.BranchRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }


        public Branch GetItem(long? id, string includeProperties)
        {
            return _contextUow.BranchRepository.Get(e => e.BranchId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public Branch GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.BranchRepository.GetAsNoTracking(e => e.BranchId == id, includeProperties: includeProperties).FirstOrDefault();
        }


        public static long GetAssetStockInBranchId()
        {

            return 499;//HO
        }

        public static long GetAssetToBeDisposedBranchId()
        {

            return GetAssetStockInBranchId();
        }

        public bool Insert(Branch item)
        {
            try
            {
                _contextUow.BranchRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Branch item)
        {
            try
            {
                _contextUow.BranchRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Branch item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.BranchRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Branch item)
        {
            if (_contextUow.TicketRepository.Get(e => e.BranchId == item.BranchId && e.IsDeleted == false).Any())
                return true;
            if (_contextUow.AssetRepository.Get(e => e.BranchId == item.BranchId && e.IsDeleted == false).Any())
                return true;
            return false;
        }

    }
}
