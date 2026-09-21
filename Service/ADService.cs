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
    public class ADService
    {
        private string domainControllerServerName = "LKISWUADC1";
        private string OUPath = "OU=NDB Users,DC=ndblk,DC=int";

        public bool Authenticate(string userName, string password)
        {
            //var activeDirectoryGroups = new List<string>();

            //using (var pc = new PrincipalContext(ContextType.Domain, domainControllerServerName))
            //{
            //    var validated = pc.ValidateCredentials(userName.Trim(), password.Trim());

            //    if (!validated) return false;
            //}
            //return true;

            //**************
            bool isValid = false;
            using (PrincipalContext pc = new PrincipalContext(ContextType.Domain, domainControllerServerName,OUPath))
            {
                // validate the credentials
                isValid = pc.ValidateCredentials(userName, password);
            }

            return isValid;

            //*********************

            //PrincipalContext pc = new PrincipalContext(ContextType.Domain, "YOUR DOMAIN");
            //return pc.ValidateCredentials(userName, "password");

        }

        //public bool Authenticate(string username, string password)
        //{
        //    bool result = false;
        //    using (DirectoryEntry _entry = new DirectoryEntry())
        //    {
        //        _entry.Username = username;
        //        _entry.Password = password;
        //        DirectorySearcher _searcher = new DirectorySearcher(_entry);
        //        _searcher.Filter = "(objectclass=user)";
        //        try
        //        {
        //            SearchResult _sr = _searcher.FindOne();
        //            string _name = _sr.Properties["displayname"][0].ToString();
        //            result = true;
        //        }
        //        catch
        //        { /* Error handling omitted to keep code short: remember to handle exceptions !*/ }
        //    }

        //    return result; //true = user authenticated!
        //}


        public User GetADDetails(string userName)
        {
            //cm_dom_ad_user cm_dom_ad_user = new cm_dom_ad_user();
            User _user = new User();
            using (var pc = new PrincipalContext(ContextType.Domain, domainControllerServerName,OUPath))
            {
                try
                {
                    UserPrincipal user = UserPrincipal.FindByIdentity(pc, IdentityType.SamAccountName, "" + domainControllerServerName + "\\" + userName);
                    DirectoryEntry entry = (DirectoryEntry)user.GetUnderlyingObject();

                    _user.EmpNo = user.SamAccountName.ToLower();
                    //_user.FullName = user.DisplayName;
                    _user.Email = user.EmailAddress;
                    _user.FullName = entry.Properties["name"].Value?.ToString();
                    _user.DesignationName = entry.Properties["Title"].Value?.ToString();
                    _user.DepartmentName = entry.Properties["department"].Value?.ToString();
                    //_user.ProImageName = entry.Properties["thumbnailPhoto"].Value?.ToString();
                    _user.ProImageName = "/Images/userimage.png";
                }
                catch (Exception ex) { }

            }

            return _user;
        }

        public Collection<User> GetUsersByEmailSearch(string filter)
        {
            Collection<User> users = new Collection<User>();
            DirectoryEntry entry = new DirectoryEntry("LDAP://" +domainControllerServerName+"/"+ OUPath);
            DirectorySearcher dSearch = new DirectorySearcher(entry);
            dSearch.Filter = "(&(objectClass=user)(objectcategory=person)(mail=*" + filter + "*))";

            foreach (SearchResult sResultSet in dSearch.FindAll())
            {
                if (sResultSet.Properties["mail"].Count > 0)
                {
                    User _user = new User();
                    _user.Email = sResultSet.Properties["mail"][0].ToString();

                    users.Add(_user);
                }
            }

            return users;
        }

        public Collection<User> GetUsersByNameSearch(string filter)
        {
            Collection<User> users = new Collection<User>();
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + domainControllerServerName + "/"+OUPath);
            DirectorySearcher dSearch = new DirectorySearcher(entry);
            dSearch.Filter = "(&(objectClass=user)(objectcategory=person)(displayname=*" + filter + "*))";

            int count = 0;
            foreach (SearchResult sResultSet in dSearch.FindAll())
            {
                if (sResultSet.Properties["name"].Count > 0 && count<=100)
                {
                    User _user = new User();
                    _user.EmpNo = sResultSet.Properties["samaccountname"][0].ToString().ToLower();
                    try
                    {
                        _user.FullName = sResultSet.Properties["displayname"][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        _user.FullName = "N/A";
                    }
                    try
                    {
                        _user.Email = sResultSet.Properties["mail"][0].ToString();
                    }
                    catch {
                        _user.Email = "N/A";
                    }
                    try
                    {
                        _user.DesignationName = sResultSet.Properties["Title"][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        _user.DesignationName = "N/A";
                    }
                    try
                    {
                        _user.DepartmentName = sResultSet.Properties["department"][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        _user.DepartmentName = "N/A";
                    }

                    users.Add(_user);
                }

                count++;
            }

            return users;
        }

        public Collection<User> GetUsersByLanId(string filter)
        {
            Collection<User> users = new Collection<User>();
            DirectoryEntry entry = new DirectoryEntry("LDAP://" + domainControllerServerName);
            DirectorySearcher dSearch = new DirectorySearcher(entry);
            dSearch.Filter = "(&(objectClass=user)(objectcategory=person)(samaccountname=*" + filter + "*))";

            int count = 0;
            foreach (SearchResult sResultSet in dSearch.FindAll())
            {
                if (sResultSet.Properties["name"].Count > 0 && count <= 100)
                {
                    User _user = new User();
                    _user.EmpNo = sResultSet.Properties["samaccountname"][0].ToString().ToLower();
                    try
                    {
                        _user.FullName = sResultSet.Properties["displayname"][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        _user.FullName = "N/A";
                    }
                    try
                    {
                        _user.Email = sResultSet.Properties["mail"][0].ToString();
                    }
                    catch
                    {
                        _user.Email = "N/A";
                    }
                    try
                    {
                        _user.DesignationName = sResultSet.Properties["Title"][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        _user.DesignationName = "N/A";
                    }
                    try
                    {
                        _user.DepartmentName = sResultSet.Properties["department"][0].ToString();
                    }
                    catch (Exception ex)
                    {
                        _user.DepartmentName = "N/A";
                    }

                    users.Add(_user);
                }

                count++;
            }

            return users;
        }
    
    
    }
}
