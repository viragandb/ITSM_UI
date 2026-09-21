using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserGroupService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserGroup> GetAll(string includeProperties)
        {
            return _contextUow.UserGroupRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserGroup GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserGroupRepository.Get(e => e.UserGroupId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public IEnumerable<UserGroup> GetUserAccess(long? branchId, long? departmentId, long? groupId, string includeProperties)
        {
            //return _contextUow.UserGroupRepository.Get(e => e.UserGroupId == id, includeProperties: includeProperties).FirstOrDefault();

           // var user = _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == userId).FirstOrDefault();
            return _contextUow.UserGroupRepository.GetAsNoTracking(e => e.IsDeleted == false
            && (e.GroupId == groupId)
             && (e.User.BranchId == branchId || e.User.UserBranches.Any(u => u.BranchId == branchId ) || branchId == 0 )
             && (e.User.DepartmentId == departmentId || e.User.UserDepartments.Any(u => u.DepartmentId == departmentId) || departmentId == 0)

             //&& (e.BranchId == branchId || branchId == 0)
             //&& (e.DepartmentId == departmentId || departmentId == 0)
             //&& ((e.BranchId == user.BranchId && e.DepartmentId == user.DepartmentId) ||
             //     (e.Branch.UserBranches.Any(u => u.EmpNo == userId) && e.Department.UserDepartments.Any(u => u.EmpNo == userId))
             //   )
            , includeProperties: includeProperties);

        }
      

        public bool Insert(UserGroup item)
        {
            try
            {
                _contextUow.UserGroupRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserGroup item)
        {
            try
            {
                _contextUow.UserGroupRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserGroup item)
        {
            try
            {
                var deleteItem = _contextUow.UserGroupRepository.GetById(item.UserGroupId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.UserGroupRepository.Delete(deleteItem);
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

        public bool HasRelationalData(UserGroup item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.UserGroupId == item.UserGroupId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public List<UserGroup> GetUserGroupsViewModels(string empNo)
        {
            var contextOfWork = new UnitOfWork();
            var userGroups = new List<UserGroup>();
            var groups =
                contextOfWork.GroupRepository.GetAsNoTracking(
                    e => e.IsDeleted == false, includeProperties: "UserGroups");
            foreach (var @group in groups)
            {
                if (group.UserGroups.Any(e => e.EmpNo == empNo && e.IsDeleted == false))
                {
                    var userGroup = group.UserGroups.Where(e => e.EmpNo == empNo && e.IsDeleted == false).FirstOrDefault();
                    userGroups.Add(new UserGroup
                    {
                        GroupId = group.GroupId,
                        IsSelected = true,
                        EmpNo = empNo,
                        Group = group,
                        UserGroupId = userGroup.UserGroupId
                    });
                }
                else
                {
                    userGroups.Add(new UserGroup
                    {
                        GroupId = group.GroupId,
                        IsSelected = false,
                        Group = group,
                        UserGroupId = 0
                    });
                }
            }


            return userGroups;
        }


        public int GetDefaultUserGroupId()
        {
            return 1;
        }
    }
}
