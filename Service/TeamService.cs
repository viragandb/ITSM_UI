using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class TeamService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<Team> GetAll()
        {
            return GetAll("");
        }
        public IEnumerable<Team> GetAll(string includeProperties)
        {
            return _contextUow.TeamRepository.GetAsNoTracking(e => e.IsDeleted == false && e.TeamId!=1, includeProperties: includeProperties);
        }
        public IEnumerable<Team> GetAllAsNoTracking(string includeProperties)
        {
            return _contextUow.TeamRepository.GetAsNoTracking(e => e.IsDeleted == false && e.TeamId!=1, includeProperties: includeProperties);
        }
        public IEnumerable<Team> GetItemsByName(string term, string includeProperties)
        {
            return _contextUow.TeamRepository.Get(e => e.IsDeleted == false
            && e.TeamName.ToUpper().Contains(term.ToUpper()), includeProperties: includeProperties);
        }
        public IEnumerable<Team> GetTeamByUserId(string userId, string includeProperties)
        {
            return _contextUow.TeamRepository.Get(e => e.IsDeleted == false && e.UserTeams.Any(u => u.EmpNo == userId)
            , includeProperties: includeProperties);
        }
        public Team GetItem(long? id, string includeProperties)
        {
            return _contextUow.TeamRepository.Get(e => e.TeamId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public Team GetItemAsNoTracking(long? id, string includeProperties)
        {
            return _contextUow.TeamRepository.GetAsNoTracking(e => e.TeamId == id, includeProperties: includeProperties).FirstOrDefault();
        }
        public string GetItemName(long? id, string includeProperties)
        {
            return _contextUow.TeamRepository.GetAsNoTracking(e => e.TeamId == id, includeProperties: includeProperties).FirstOrDefault().TeamName;
        }

        public bool Insert(Team item)
        {
            try
            {
                _contextUow.TeamRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(Team item)
        {
            try
            {
                _contextUow.TeamRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(Team item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.TeamRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(Team item)
        {
            if (_contextUow.UserTeamRepository.Get(e => e.TeamId == item.TeamId && e.IsDeleted == false).Any())
                return true;

            return false;
        }

        public int GetTeamIdDefault()
        {

            return 1;
        }

        public static int GetTeamIdHelpDesk()
        {

            return 2;
        }

        public int GetTeamIdAppSupport()
        {

            return 1;
        }
        public int GetTeamIdDev()
        {

            return 2;
        }
        public int GetTeamIdITOperation()
        {

            return 9;
        }
        public int GetTeamIdNetwork()
        {

            return 11;
        }
        public int GetTeamIdDBAdmin()
        {

            return 12;
        }
        public int GetTeamIdAVP()
        {

            return 5;
        }
        public int GetTeamIdITSVP()
        {

            return 13;
        }

        public int GetTeamIdDCEO()
        {

            return 22;
        }



        public int GetTeamIdRisk()
        {

            return 19;
        }

        public int GetTeamIdHR()
        {

            return 20;
        }

        public int GetTeamIdITSecHead()
        {

            return 21;
        }

        public int GetTeamIdITSec()
        {

            return 7;
        }

        public int GetTeamIdITRemoteNet()
        {

            return 4;
        }
    }
}
