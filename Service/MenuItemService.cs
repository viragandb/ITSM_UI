using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MenuItemService
    {
        public IEnumerable<MenuItem> GetMenuItems(string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuItemRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<MenuItem> GetMenuItems(long? menuId, string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuItemRepository.GetAsNoTracking(e => e.IsDeleted == false && e.MenuId == menuId, includeProperties: includeProperties);
        }
        public IEnumerable<MenuItem> GetMenuItems(long? menuId, string userId, string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            //return contextOfWork.MenuRepository.GetAsNoTracking(e => e.IsDeleted == false && e.Area == areaName 
            // && e.MenuItems.Any(a => a.MenuItemFunctions.Any(d => d.AccessControls.Any(f => f.Group.UserGroups.Any(g => g.EmpNo == userId)))), includeProperties: includeProperties).OrderBy(e => e.MenuOrder);

            return contextOfWork.MenuItemRepository.GetAsNoTracking(e => e.IsDeleted == false && e.MenuId == menuId
            && e.MenuItemFunctions.Any(a => a.AccessControls.Any(f => f.Group.UserGroups.Any(g => g.EmpNo == userId)))
            , includeProperties: includeProperties);
        }

        public MenuItem GetMenuItem(string mvcArea, string mvcController, string inclueProperties)
        {
            var contextOfWork = new UnitOfWork();

            return
                contextOfWork.MenuItemRepository.GetAsNoTracking(
                    e => e.Area == mvcArea && e.Controller == mvcController,
                    includeProperties: inclueProperties).FirstOrDefault();
        }

    }
}
