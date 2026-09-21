using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
     public class ChangeProblemService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<ChangeProblem> GetAll(string includeProperties)
        {
            return _contextUow.ChangeProblemRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<ChangeProblem> GetChangeProblemByChangeId(long? id, string includeProperties)
        {
            return _contextUow.ChangeProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
            && e.ChangeId == id, includeProperties: includeProperties);
        }

        public IEnumerable<ChangeProblem> GetChangesWithProblems(DateTime sDate, DateTime eDate, string includeProperties)
        {

            return _contextUow.ChangeProblemRepository.GetAsNoTracking(e => e.IsDeleted == false
           && e.Change.RequestedDate >= sDate && e.Change.RequestedDate < eDate
            && e.Change.ChangeRequestType.IsKPIApplicable == true
           , includeProperties: includeProperties);


        }

        public ChangeProblem GetItem(long? id, string includeProperties)
        {
            return _contextUow.ChangeProblemRepository.Get(e => e.ChangeProblemId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public ChangeProblem GetItem(long? ChangeId, long? ProblemId, string includeProperties)
        {
            return _contextUow.ChangeProblemRepository.Get(e => e.ChangeId == ChangeId && e.ProblemId == ProblemId && e.IsDeleted == false, includeProperties: includeProperties).FirstOrDefault();
        }

        public bool Insert(ChangeProblem item)
        {
            try
            {
                _contextUow.ChangeProblemRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(ChangeProblem item)
        {
            try
            {
                _contextUow.ChangeProblemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(ChangeProblem item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.ChangeProblemRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool IsAvailable(ChangeProblem item)
        {
            if (_contextUow.ChangeProblemRepository.Get(e => e.ChangeId == item.ChangeId && e.ProblemId == item.ProblemId && e.IsDeleted == false).Any())
                return true;

            return false;
        }

        public bool HasRelationalData(ChangeProblem item)
        {
            //if (_contextUow.ChangeProblemRepository.Get(e => e.ChangeProblemId == item.ChangeProblemId && e.IsDeleted == false).Any())
            //    return true;

            return false;
        }


    }
}
