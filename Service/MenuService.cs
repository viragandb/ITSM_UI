using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MenuService
    {
        public IEnumerable<Menu> GetMenus(string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        //public IEnumerable<Menu> GetMenus(long? moduleId, string includeProperties)
        //{
        //    var contextOfWork = new UnitOfWork();

        //    return contextOfWork.MenuRepository.GetAsNoTracking(e => e.IsDeleted == false && e.ModuleId == moduleId, includeProperties: includeProperties);
        //}

        public IEnumerable<Menu> GetMenus(string userId, string areaName, string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuRepository.GetAsNoTracking(e => e.IsDeleted == false && e.Area == areaName && e.MenuItems.Any(a => a.MenuItemFunctions.Any(d => d.AccessControls.Any(f => f.Group.UserGroups.Any(g => g.EmpNo == userId)))), includeProperties: includeProperties).OrderBy(e => e.MenuOrder);
        }


        public IEnumerable<Menu> GetMenus(string areaName, string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuRepository.GetAsNoTracking(e => e.IsDeleted == false && e.Area == areaName, includeProperties: includeProperties).OrderBy(e => e.MenuOrder);
        }
    }
}
