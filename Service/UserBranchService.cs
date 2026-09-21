using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserBranchService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserBranch> GetAll(string includeProperties)
        {
            return _contextUow.UserBranchRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserBranch GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserBranchRepository.Get(e => e.UserBranchId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public long GetUserBranchId(string userId, string includeProperties)
        {

            return _contextUow.UserBranchRepository.Get(e => e.IsDeleted == false && e.EmpNo == userId, includeProperties: includeProperties).FirstOrDefault().BranchId;
        }



        public bool Insert(UserBranch item)
        {
            try
            {
                _contextUow.UserBranchRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserBranch item)
        {
            try
            {
                _contextUow.UserBranchRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserBranch item)
        {
            try
            {
                var deleteItem = _contextUow.UserBranchRepository.GetById(item.UserBranchId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.UserBranchRepository.Delete(deleteItem);
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


        public bool HasRelationalData(UserBranch item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.UserTeamId == item.UserTeamId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }


        public List<UserBranch> GetUserBranchesViewModels(string empNo)
        {
            var contextOfWork = new UnitOfWork();
            var userBranches = new List<UserBranch>();
            var branches = contextOfWork.BranchRepository.GetAsNoTracking(
                    e => e.IsDeleted == false, includeProperties: "UserBranches").OrderBy(o=> o.Name);
            foreach (var @item in branches)
            {
                if (item.UserBranches.Any(e => e.EmpNo == empNo && e.IsDeleted == false))
                {
                    var userBranch = @item.UserBranches.Where(e => e.EmpNo == empNo && e.IsDeleted == false).FirstOrDefault();
                    userBranches.Add(new UserBranch
                    {
                        BranchId = item.BranchId,
                        IsSelected = true,
                        EmpNo = empNo,
                        Branch = item,
                        UserBranchId = userBranch.UserBranchId
                    });
                }
                else
                {
                    userBranches.Add(new UserBranch
                    {
                        BranchId = item.BranchId,
                        IsSelected = false,
                        Branch = item,
                        UserBranchId = 0
                    });
                }
            }


            return userBranches;
        }
    }
}
