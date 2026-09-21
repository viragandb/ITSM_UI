using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class MenuItemFunctionService
    {

        public IEnumerable<MenuItemFunction> GetMenuItemFunctions(string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuItemFunctionRepository.GetAsNoTracking(e => e.IsDeleted == false,
                includeProperties: includeProperties);
        }

        public IEnumerable<MenuItemFunction> GetMenuItemFunctions(long? menuItemId, string includeProperties)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.MenuItemFunctionRepository.GetAsNoTracking(e => e.IsDeleted == false && e.MenuItemId == menuItemId,
                includeProperties: includeProperties);
        }

        public MenuItemFunction GetMenuItemFunction(string mvcArea, string mvcController, string mvcAction, string inclueProperties)
        {
            var contextOfWork = new UnitOfWork();

            return
                contextOfWork.MenuItemFunctionRepository.GetAsNoTracking(
                    e => e.Area == mvcArea && e.Controller == mvcController && e.Action == mvcAction,
                    includeProperties: inclueProperties).FirstOrDefault();
        }


    }
}
