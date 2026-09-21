using Data;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Service
{
    public class UserService
    {
        private readonly UnitOfWork _contextUow = new UnitOfWork();
        private ADService _adService = new ADService();

        public bool IsValidLogin(UserLogin userLogin)
        {
            return _adService.Authenticate(userLogin.UserName.ToLower(), userLogin.Password);
        }

        public string GetProfileImageDefault()
        {
            return "/Images/userimage.png";
        }

        public static string GetUserId_IT_VP()
        {
            return "indika_7655";
        }

        public static string GetUserId_IT_Infra_AVP()
        {
            return "amila_8924";
        }

        public User GetADUser(string empNo)
        {
            return _adService.GetADDetails(empNo);


        }
        public IEnumerable<User> GetUserByTeamId(long? teamId, string includeProperties)
        {
            return _contextUow.UserRepository.Get(e => e.IsDeleted == false
            && e.UserStatus == UserStatusEnum.Active
            && e.UserTeams.Any(u => u.TeamId == teamId), includeProperties: includeProperties);
        }
      

        public IEnumerable<User> GetUsersByName(string term, string includeProperties)
        {
            return _contextUow.UserRepository.Get(e => e.IsDeleted == false
            && e.FullName.ToUpper().Contains(term.ToUpper())
            && e.UserStatus == UserStatusEnum.Active
            , includeProperties: includeProperties);
        }


        public IEnumerable<User> GetAll(string includeProperties)
        {
            return _contextUow.UserRepository.Get(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public IEnumerable<User> GetAsNoTrackingAll(string includeProperties)
        {
            return _contextUow.UserRepository.GetAsNoTracking(e => e.IsDeleted == false, includeProperties: includeProperties);
        }

        public void UpdateContactNo(string empNo, string contactNo)
        {
            var user = _contextUow.UserRepository.Get(e => e.EmpNo == empNo, includeProperties: "").FirstOrDefault();
            user.ContactNo = contactNo;
            Update(user);

        }
        //public User GetItem(long? id, string includeProperties)
        //{
        //    return _contextUow.UserRepository.Get(e => e.UserId == id, includeProperties: includeProperties).FirstOrDefault();
        //}

        public User GetUserByEmpNo(string empNo, string includeProperties)
        {
            return _contextUow.UserRepository.Get(e => e.EmpNo == empNo, includeProperties: includeProperties).FirstOrDefault();
        }
       
        public User GetUserByEmpNo_UpdateByAD(string empNo, string updatedby, DateTime updatedDate, string includeProperties)
        {
            var user = _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == empNo, includeProperties: includeProperties).FirstOrDefault();
            if (user == null)
            {
                user = _adService.GetADDetails(empNo);
                user.EmpNo = user.EmpNo.ToLower();
                user.UpdatedDate = updatedDate;
                user.UpdatedBy = updatedby;
                user.LastActiveDate = updatedDate;
                user.UserStatus = UserStatusEnum.Active;
                Insert(user);

            }
            return user;
        }

        public User GetAsNoTrackingUserByEmpNo(string empNo, string includeProperties)
        {
            return _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == empNo, includeProperties: includeProperties).FirstOrDefault();
        }


        public bool IsUserExist(string empNo)
        {
            return _contextUow.UserRepository.GetAsNoTracking(e => e.EmpNo == empNo, includeProperties: "").Any();
        }

        public bool Insert(User item)
        {
            try
            {
                item.EmpNo = item.EmpNo.ToLower();
                _contextUow.UserRepository.Insert(item);
                _contextUow.Save();

                UserGroupService _userGroupService = new UserGroupService();
                UserGroup group = new UserGroup();
                group.EmpNo = item.EmpNo;
                group.GroupId = _userGroupService.GetDefaultUserGroupId();
                group.UpdatedBy = item.UpdatedBy;
                group.UpdatedDate = item.UpdatedDate;
                _userGroupService.Insert(group);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Update(User item)
        {
            try
            {
                _contextUow.UserRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Delete(User item)
        {
            try
            {
                item.IsDeleted = true;
                _contextUow.UserRepository.Update(item);
                _contextUow.Save();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool HasRelationalData(User item)
        {
            //if (_contextUow.BoardMemberRepository.Get(e => e.UserId == item.UserId && e.IsDeleted == false).Any())
            //    return true;

            return false; ;
        }

        public bool IsUserLoged(string userId, bool rememberMe, DateTime dateTime)
        {
            bool res = false;

            UserService _userService = new UserService();
            var user = _userService.GetUserByEmpNo(userId, "");

            if (user.UserStatus == UserStatusEnum.Active)
            {


                int timeOut = rememberMe ? 525600 : 30;
                var formTicket = new FormsAuthenticationTicket(user.EmpNo, false, timeOut);
                string formEncrypt = FormsAuthentication.Encrypt(formTicket);

                var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, formEncrypt)
                {
                    Expires = DateTime.Now.AddMinutes(timeOut),
                    HttpOnly = true
                };

                HttpContext.Current.Response.Cookies.Add(cookie);
                //httpResponse.Cookies.Add(cookie);

                HttpSessionState mySession = ((HttpSessionState)HttpContext.Current.Session);

                mySession["UserId"] = user.EmpNo;
                mySession["UserName"] = user.FullName;
                //mySession["CompanyId"] = user.CompanyId;
                //mySession["CompanyName"] = user.CompanyName;
                if (!string.IsNullOrEmpty(user.ProImageName))
                {
                    mySession["ProfileImage"] = user.ProImageName;
                }
                user.LastActiveDate = dateTime;
                _userService.Update(user);
                res = true;
            }

            return res;

        }
        public bool HasAccessToFunction(string includeProperties, string userId, string area = null,
        string controller = null,
        string action = null)
        {
            var contextOfWork = new UnitOfWork();

            return contextOfWork.AccessControlRepository.GetAsNoTracking(
                e =>
                    e.MenuItemFunction.Controller.Trim() == controller.Trim() &&
                    e.MenuItemFunction.Area.Trim() == area.Trim() &&
                    e.MenuItemFunction.Action.Trim() == action &&
                    e.Group.UserGroups.Any(w => w.EmpNo == userId),
                includeProperties: "Group.UserGroups,MenuItemFunction").Any();
            //return false;
        }
    }
}

