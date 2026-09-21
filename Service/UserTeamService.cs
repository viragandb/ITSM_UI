using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class UserTeamService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();

        public IEnumerable<UserTeam> GetAll(string includeProperties)
        {
            return _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public UserTeam GetItem(long? id, string includeProperties)
        {
            return _contextUow.UserTeamRepository.Get(e => e.UserTeamId == id, includeProperties: includeProperties).FirstOrDefault();
        }

        public long GetUserTeamId(string userId, string includeProperties)
        {

            return _contextUow.UserTeamRepository.Get(e => e.IsDeleted == false && e.EmpNo == userId, includeProperties: includeProperties).FirstOrDefault().TeamId;
        }


     
        public bool Insert(UserTeam item)
        {
            try
            {
                _contextUow.UserTeamRepository.Insert(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(UserTeam item)
        {
            try
            {
                _contextUow.UserTeamRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(UserTeam item)
        {
            try
            {
                var deleteItem = _contextUow.UserTeamRepository.GetById(item.UserTeamId);

                if (item != null)
                {
                    // item.IsDeleted = true;
                    _contextUow.UserTeamRepository.Delete(deleteItem);
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

        public bool IsTeamUser(string userId,long teamId)
        {
            if (_contextUow.UserTeamRepository.Get(e => e.EmpNo==userId && e.TeamId == teamId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }
        public bool IsTeamUser(string userId)
        {
            if (_contextUow.UserTeamRepository.Get(e => e.EmpNo == userId && e.IsDeleted == false).Any())
                return true;

            return false; ;
        }

        public bool IsTeamUserIT(string userId)
        {
            if (_contextUow.UserTeamRepository.Get(e => e.EmpNo == userId && e.TeamId==10 ).Any())
                return true;

            return false; ;
        }

        

        public bool HasRelationalData(UserTeam item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.UserTeamId == item.UserTeamId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public List<UserTeam> GetUserTeamsViewModels(string empNo)
        {
            var contextOfWork = new UnitOfWork();
            var userTeams = new List<UserTeam>();
            var teams = contextOfWork.TeamRepository.GetAsNoTracking(
                    e => e.IsDeleted == false, includeProperties: "UserTeams");
            foreach (var @team in teams)
            {
                if (team.UserTeams.Any(e => e.EmpNo == empNo && e.IsDeleted == false))
                {
                    var userTeam = team.UserTeams.Where(e => e.EmpNo == empNo && e.IsDeleted == false).FirstOrDefault();
                    userTeams.Add(new UserTeam
                    {
                        TeamId = team.TeamId,
                        IsSelected = true,
                        EmpNo = empNo,
                        Team = team,
                        UserTeamId = userTeam.UserTeamId
                    });
                }
                else
                {
                    userTeams.Add(new UserTeam
                    {
                        TeamId = team.TeamId,
                        IsSelected = false,
                        Team = team,
                        UserTeamId = 0
                    });
                }
            }


            return userTeams;
        }

    }

}
