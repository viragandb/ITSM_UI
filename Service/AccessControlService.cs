using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class AccessControlService
    {
        public IEnumerable<AccessControl> GetUserGroupAccess(string includeProperties, long? groupId)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.AccessControlRepository.Get(e => (e.GroupId == groupId), includeProperties: includeProperties);
        }

        public bool DeleteAccessControl(AccessControl accessControl)
        {
            var contextOfWork = new UnitOfWork();
            var accControl = contextOfWork.AccessControlRepository.GetById(accessControl.AccessControlId);
            try
            {
                if (accControl != null)
                {
                    contextOfWork.AccessControlRepository.Delete(accControl);
                    contextOfWork.Save();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool InsertAccessControl(AccessControl accessControl)
        {
            var contextOfWork = new UnitOfWork();

            try
            {

                contextOfWork.AccessControlRepository.Insert(accessControl);
                contextOfWork.Save();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void GrantAccess(long groupId, string accessControls,string userId)
        {
            var existAccessControls = GetUserGroupAccess("", groupId);

            foreach (var existAccessControl in existAccessControls)
            {
                DeleteAccessControl(existAccessControl);
            }

            var accessIds = accessControls.Split(',');

            foreach (var accessId in accessIds)
            {
                try
                {
                    var accessControl = new AccessControl()
                    {
                        MenuItemFunctionId = Convert.ToInt64(accessId),
                        GroupId = groupId,
                        UpdatedBy = userId,
                        UpdatedDate = DateTimeService.GetUserDate()

                    };

                    InsertAccessControl(accessControl);
                }
                catch (Exception)
                {

                }
            }
        }

        public IList<Menu> GetMenus(string empNo, string includeProperties)
        {
            var contextOfWork = new UnitOfWork();


            return (
            contextOfWork.MenuRepository.GetAsNoTracking(
                e => e.IsDeleted == false &&
                e.MenuItems.Any(a => a.MenuItemFunctions.Any(s => s.AccessControls.Any(q => q.Group.UserGroups.Any(r => r.EmpNo == empNo)
                                                                                      )
                                                            )
                                ),
                orderBy: m => m.OrderBy(p => p.MenuOrder), includeProperties: includeProperties)).ToList();

        }
        public IList<Menu> GetMenus(string includeProperties)
        {
            var contextOfWork = new UnitOfWork();


            return (
            contextOfWork.MenuRepository.GetAsNoTracking(
                e => e.IsDeleted == false,
                orderBy: m => m.OrderBy(p => p.MenuOrder), includeProperties: includeProperties)).ToList();
        }

        public IEnumerable<AccessControl> GetAccessControls(long groupId)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.AccessControlRepository.GetAsNoTracking(e => e.GroupId == groupId);
        }
    }
}
