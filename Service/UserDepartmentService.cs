using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserDepartmentService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserDepartment> GetAll(string includeProperties)
        {
            return _contextUow.UserDepartmentRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserDepartment GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserDepartmentRepository.Get(e => e.UserDepartmentId == id, includeProperties: includeProperties).FirstOrDefault();
        }



        public bool Insert(UserDepartment item)
        {
            try
            {
                _contextUow.UserDepartmentRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserDepartment item)
        {
            try
            {
                _contextUow.UserDepartmentRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserDepartment item)
        {
            try
            {
                var deleteItem = _contextUow.UserDepartmentRepository.GetById(item.UserDepartmentId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.UserDepartmentRepository.Delete(deleteItem);
                    _contextUow.Save();

                    return true;
                }
                else
                    return false;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public bool HasRelationalData(UserDepartment item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.UserTeamId == item.UserTeamId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }


        public List<UserDepartment> GetUserDepartmentsViewModels(string empNo)
        {
            var contextOfWork = new UnitOfWork();
            var userDepts = new List<UserDepartment>();
            var depts = contextOfWork.DepartmentRepository.GetAsNoTracking(
                    e => e.IsDeleted == false, includeProperties: "UserDepartments").OrderBy(o => o.DepartmentName);
            foreach (var item in depts)
            {
                if (item.UserDepartments.Any(e => e.EmpNo == empNo && e.IsDeleted == false))
                {
                    var userDept = item.UserDepartments.Where(e => e.EmpNo == empNo && e.IsDeleted == false).FirstOrDefault();
                    userDepts.Add(new UserDepartment
                    {
                        DepartmentId = item.DepartmentId,
                        IsSelected = true,
                        EmpNo = empNo,
                        Department = item,
                        UserDepartmentId = userDept.UserDepartmentId
                    });
                }
                else
                {
                    userDepts.Add(new UserDepartment
                    {
                        DepartmentId = item.DepartmentId,
                        IsSelected = false,
                        Department = item,
                        UserDepartmentId = 0
                    });
                }
            }

            return userDepts;
        }


     


    }
}
