using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BranchDepartmentService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<BranchDepartment> GetAll(string includeProperties)
        {
            return _contextUow.BranchDepartmentRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
       
        public IEnumerable<BranchDepartment> GetItemsByBranchId(long? branchId, string includeProperties)
        {
            return _contextUow.BranchDepartmentRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.BranchId== branchId
            , includeProperties: includeProperties);

        }

        public bool IsUpdated(long? departmentId, long? branchId)
        {
            return _contextUow.BranchDepartmentRepository.GetAsNoTracking(e => e.DepartmentId == departmentId && e.BranchId == branchId, includeProperties: "").Any();
        }

        public BranchDepartment GetItem(long? id, string includeProperties)
        {
            return _contextUow.BranchDepartmentRepository.Get(e => e.BranchDepartmentId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(BranchDepartment item)
        {
            try
            {
                _contextUow.BranchDepartmentRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(BranchDepartment item)
        {
            try
            {
                _contextUow.BranchDepartmentRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //public bool Delete(BranchDepartment item)
        //{
        //    try
        //    {
        //        item.IsDeleted = true;
        //        _contextUow.BranchDepartmentRepository.Update(item);
        //        _contextUow.Save();
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        return false;
        //    }
        //}

        public bool Delete(long? id)
        {
            try
            {
                _contextUow.BranchDepartmentRepository.Delete(id);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(BranchDepartment item)
        {
            //if (_contextUow.RequestTypeRepository.Get(e => e.BranchDepartmentId == item.BranchDepartmentId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

    }
}
