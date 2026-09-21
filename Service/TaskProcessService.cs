using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TaskProcessService
    {

        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public TaskProcess GetItem(string code, DateTime date, string includeProperties)
        {
            return _contextUow.TaskProcessRepository.Get(e => e.Code == code && e.TaskDate == date, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool IsTaskProcessed(string code, DateTime date)
        {
            if (_contextUow.TaskProcessRepository.Get(e => e.Code == code && e.TaskDate == date).Any())
                return true;

            return false;
        }
        public bool Insert(TaskProcess item)
        {
            try
            {
                _contextUow.TaskProcessRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(TaskProcess item)
        {
            try
            {
                _contextUow.TaskProcessRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public string GetCodeTaskCreation()
        {
            return "TUC";
        }
    }
}
