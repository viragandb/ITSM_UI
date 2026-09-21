using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class DepartmentService
    {

        private readonly UnitOfWork _contextUow = new UnitOfWork();
        public IEnumerable<Department> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<Department> GetAll(string includeProperties)
        {
            return _contextUow.DepartmentRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<Department> GetItemsByBranchId(long? branchId,string includeProperties)
        {
            return _contextUow.DepartmentRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.BranchDepartments.Any(b=>b.BranchId==branchId), includeProperties: includeProperties);
        }

        public IEnumerable<Department> GetItemsByBranchIdUser(long? branchId, string userId, string includeProperties)
        {
            var user = _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == userId).FirstOrDefault();

            return _contextUow.DepartmentRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.BranchDepartments.Any(b => b.BranchId == branchId)
            && (e.DepartmentId==user.DepartmentId ||  e.UserDepartments.Any(u => u.EmpNo == userId)  )
            , includeProperties: includeProperties);
        }

        public Department GetItem(long? id, string includeProperties)
        {
            return _contextUow.DepartmentRepository.Get(e => e.DepartmentId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public Department GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.DepartmentRepository.GetAsNoTracking(e => e.DepartmentId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public static long GetAssetStockInDeptId()
        {
            return 358;//It Store
        }

          public static long GetDeptIdBranch()
        {
            return 266;//Branch
        }
        public static long GetAssetToBeDisposedDeptId()
        {
            return GetAssetStockInDeptId();
        }
        public bool Insert(Department item)
        {
            try
            {
                _contextUow.DepartmentRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Department item)
        {
            try
            {
                _contextUow.DepartmentRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Department item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.DepartmentRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Department item)
        {
            //if (_contextUow.RequestTypeRepository.Get(e => (e.ProcessingUnit1_DeptId == item.DepartmentId || e.Checker_DeptId == item.DepartmentId) && e.IsDeleted == false).Any())
            //    return true;
            if (_contextUow.AssetRepository.Get(e => e.DepartmentId == item.DepartmentId && e.IsDeleted == false).Any())
                return true;
            return false;
        }

        public IEnumerable<Department> GetDepartmentsForAUser(string empNo)
        {
            return _contextUow.DepartmentRepository.Get(r => r.UserDepartments.Any(u => u.EmpNo == empNo));
        }

    }
}
