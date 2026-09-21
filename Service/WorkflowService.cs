using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class WorkflowService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Workflow> GetAll(string includeProperties)
        {
            return _contextUow.WorkflowRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }
        public IEnumerable<Workflow> GetItemsByType(WorkflowTypeEnum type,string includeProperties)
        {
            return _contextUow.WorkflowRepository.GetAsNoTracking(e => e.IsDeleted == false 
            && e.WorkflowType== type, includeProperties: includeProperties);
        }
        public Workflow GetItem(long? id, string includeProperties)
        {
            return _contextUow.WorkflowRepository.Get(e => e.WorkflowId == id, includeProperties: includeProperties).FirstOrDefault();
        }


        public Workflow GetAsNoTrackingItem(long? id, string includeProperties)
        {
            return _contextUow.WorkflowRepository.GetAsNoTracking(e => e.WorkflowId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(Workflow item)
        {
            try
            {
                _contextUow.WorkflowRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Workflow item)
        {
            try
            {
                _contextUow.WorkflowRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Workflow item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.WorkflowRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Workflow item)
        {
            if (_contextUow.AccessRequestRepository.Get(e => e.WorkflowId == item.WorkflowId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }


    }
}
