using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class WorkflowLevelLevelService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<WorkflowLevel> GetAll(string includeProperties)
        {
            return _contextUow.WorkflowLevelRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<WorkflowLevel> GetItems(long? id,string includeProperties)
        {
            return _contextUow.WorkflowLevelRepository.GetAsNoTracking(e => e.IsDeleted == false && e.WorkflowId==id, includeProperties: includeProperties);
        }

        public WorkflowLevel GetItem(long? id, string includeProperties)
        {
            return _contextUow.WorkflowLevelRepository.Get(e => e.WorkflowLevelId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public WorkflowLevel GetItem(long? workflowId,int levelNo, string includeProperties)
        {
            return _contextUow.WorkflowLevelRepository.Get(e => e.WorkflowId == workflowId && e.LevelNo== levelNo
            , includeProperties: includeProperties).FirstOrDefault();
        }

        public WorkflowLevel GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.WorkflowLevelRepository.GetAsNoTracking(e => e.WorkflowLevelId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(WorkflowLevel item)
        {
            try
            {
                _contextUow.WorkflowLevelRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(WorkflowLevel item)
        {
            try
            {
                _contextUow.WorkflowLevelRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(long id)
        {
            try
            {
                
                _contextUow.WorkflowLevelRepository.Delete(id);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(WorkflowLevel item)
        {
            //if (_contextUow.RequestTypeRepository.Get(e => e.WorkflowLevelId == item.WorkflowLevelId && e.IsDeleted == false).Any())
            return true;

            return false; ;
        }


    }
}
