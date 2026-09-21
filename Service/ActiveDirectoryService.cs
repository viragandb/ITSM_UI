using Domain;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ActiveDirectoryService
    {

        public bool Authenticate(string userName, string password)
        {
            var activeDirectoryGroups = new List<string>();

            using (var pc = new PrincipalContext(ContextType.Domain, "LKISWUADC1"))
            {
                var validated = pc.ValidateCredentials(userName.Trim(), password.Trim());

                if (!validated) return false;
            }

            return true;
        }


        public User GetEmployee(string empNo)
        {
            User user = new User();
            using (var pc = new PrincipalContext(ContextType.Domain, "LKISWUADC1"))
            {
                UserPrincipal Getuser = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, "" + "LKISWUADC1" + "\\" + empNo);
                if (Getuser == null)
                {
                    return null;
                }

                DirectoryEntry entry = (DirectoryEntry)Getuser.GetUnderlyingObject();

                
                user.EmpNo = Getuser.SamAccountName;
                user.FullName = Getuser.DisplayName;
                user.Email = Getuser.EmailAddress;
                user.DesignationName = entry.Properties["Title"].Value?.ToString();

                //user.CompanyId = 1;
                //user.CompanyName = "National Development Bank PLC";
                //user.DepartmentId = 34;
                //user.DepartmentName = "Other";
            }



            return user;

        }


        public List<User> SearchEmployee(string empNo)
        {
            Collection<User> user = new Collection<User>();
            using (var pc = new PrincipalContext(ContextType.Domain, "LKISWUADC1"))
            {

                UserPrincipal up = new UserPrincipal(pc);
                up.SamAccountName = "*" + empNo + "*";
                PrincipalSearcher searcher = new PrincipalSearcher(up);
                var results = searcher.FindAll();


                foreach (var item in results)
                {
                    User luser = new User();
                    UserPrincipal Getuser = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, "" + "LKISWUADC1" + "\\" + item.SamAccountName);
                    DirectoryEntry entry = (DirectoryEntry)Getuser.GetUnderlyingObject();


                    luser.EmpNo = Getuser.SamAccountName;
                    luser.FullName = Getuser.DisplayName;
                    luser.Email = Getuser.EmailAddress;
                    luser.DesignationName = entry.Properties["Title"].Value?.ToString();

                    user.Add(luser);
                }
            }


            return user.ToList();

        }


        public string GetHRISDefaultUserImageUrl()
        {
            return "";
        }
    }
}
