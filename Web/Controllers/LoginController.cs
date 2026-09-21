using Domain;
using log4net;
using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Web.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserService _service = new UserService();
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoginController));
        // GET: Login
        public ActionResult Index(int? nav)
        {

            var item = new UserLogin();
            if (nav != null)
                NavAction(nav);

            try
            {
                string adName = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.ToString();
                if (adName != "")
                {

                    string[] split = adName.Split('\\');
                    item.UserName = split[1].ToLower();
                }

            }
            catch (Exception ex) { }

            return View(item);

        }


        [HttpPost]
        public ActionResult Index(UserLogin userLogin)
        {
            try
            {
                if (ModelState.IsValid)
                {

                    if (_service.IsValidLogin(userLogin))
                        //if (true)
                        {

                            var user = _service.GetUserByEmpNo(userLogin.UserName, "Branch,Department");

                        if (user == null)
                        {
                            UserService _userService = new UserService();
                            user = _service.GetADUser(userLogin.UserName.ToLower());
                            if (user.EmpNo != null)
                            {
                                user.UpdatedBy = user.EmpNo;
                                user.UpdatedDate = DateTimeService.GetUserDate();
                                user.LastActiveDate = DateTimeService.GetUserDate();
                                user.UserStatus = UserStatusEnum.Active;
                                _userService.Insert(user);
                            }
                            else
                            {
                                ModelState.AddModelError("Password", "You are not authorized to access the system, Please contact the system administrator");
                                return View();
                            }
                        }


                        if (user.UserStatus == UserStatusEnum.Active)
                        {

                            int timeOut = userLogin.RememberMe ? 525600 : 30;
                            var formTicket = new FormsAuthenticationTicket(user.EmpNo, userLogin.RememberMe, timeOut);
                            string formEncrypt = FormsAuthentication.Encrypt(formTicket);

                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, formEncrypt)
                            {
                                Expires = DateTime.Now.AddMinutes(timeOut),
                                HttpOnly = true
                            };

                            Response.Cookies.Add(cookie);

                            Session["UserId"] = user.EmpNo.ToLower();
                            Session["UserName"] = user.FullName;
                            //Session["Department"] = user.DepartmentName;
                            Session["Designation"] = user.DesignationName;

                            if (user.BranchId != null)
                            {
                                Session["Branch"] = user.Branch.Name;

                            }

                            if (user.DepartmentId != null)
                            {
                                Session["Department"] = user.Department.DepartmentName;

                            }

                            if (!string.IsNullOrEmpty(user.ProImageName))
                            {
                                Session["ProfileImage"] = user.ProImageName;
                            }

                            user.LastActiveDate = DateTimeService.GetUserDate();
                            _service.Update(user);

                            //string auth = "";
                            //try
                            //{
                            //    auth = Session["Auth"].ToString();
                            //}
                            //catch (Exception ex) { }
                            //if (auth != "")
                            //    return RedirectToAction("URLLogin", "Login", new { key = auth });
                            //else
                            //    return RedirectToAction("Index", "Home");

                            Log.Info(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " - " + Session["UserId"].ToString());
                            return RedirectToAction("Index", "Home");

                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Login Failed, Please try again later!");

                        }

                    }
                    else
                    {

                        ModelState.AddModelError("Password", "The credentials you’ve entered are incorrect. Please try again!");
                    }
                }
                return View();
            }
            catch (Exception ex)
            {

                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(), ex);
                return View();

            }
        }


        [HttpGet]
        public ActionResult ADLogin()
        {
            try
            {

                string adName = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.ToString();
                if (adName != "")
                {
                    string[] split = adName.Split('\\');
                    var userName = split[1].ToLower();

                    var user = _service.GetUserByEmpNo(userName, "Branch,Department");
                    if (user == null)
                    {
                        UserService _userService = new UserService();
                        user = _service.GetADUser(userName.ToLower());
                        if (user.EmpNo != null)
                        {
                            user.UpdatedBy = user.EmpNo;
                            user.UpdatedDate = DateTimeService.GetUserDate();
                            user.LastActiveDate = DateTimeService.GetUserDate();
                            user.UserStatus = UserStatusEnum.Active;
                            _userService.Insert(user);
                        }
                        else
                        {
                            ModelState.AddModelError("Password", "You are not authorized to access the system, Please contact the system administrator");
                            return View();
                        }
                    }


                    if (user.UserStatus == UserStatusEnum.Active)
                    {

                        int timeOut = 30;
                        var formTicket = new FormsAuthenticationTicket(user.EmpNo, false, timeOut);
                        string formEncrypt = FormsAuthentication.Encrypt(formTicket);

                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, formEncrypt)
                        {
                            Expires = DateTime.Now.AddMinutes(timeOut),
                            HttpOnly = true
                        };

                        Response.Cookies.Add(cookie);

                        Session["UserId"] = user.EmpNo;
                        Session["UserName"] = user.FullName;
                        //Session["Department"] = user.DepartmentName;
                        Session["Designation"] = user.DesignationName;

                        if (user.BranchId != null)
                        {
                            Session["Branch"] = user.Branch.Name;

                        }
                        if (user.DepartmentId != null)
                        {
                            Session["Department"] = user.Department.DepartmentName;

                        }

                        if (!string.IsNullOrEmpty(user.ProImageName))
                        {
                            Session["ProfileImage"] = user.ProImageName;
                        }

                        user.LastActiveDate = DateTimeService.GetUserDate();
                        _service.Update(user);

                        Log.Info(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " - " + Session["UserId"].ToString());
                        return RedirectToAction("Index", "Home");

                    }
                    else
                    {
                        ModelState.AddModelError("Password", "Login Failed, Please try again later!");

                    }
                }


            }
            catch (Exception ex)
            {
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(), ex);
            }

            return RedirectToAction("Index", "Login");

        }


        //[HttpGet]
        //public ActionResult Auth(string key, int? nav)
        //{
        //    try
        //    {
        //        if (key != "")
        //        {
        //            if (key.All(Char.IsLetterOrDigit))
        //            {
        //                string adName = GlobalStaticService.Decrypt(key);
        //                var userName = adName;
        //                var user = _service.GetUserByEmpNo(userName, "Branch,Department");
        //                if (user != null)
        //                {
        //                    if (user.UserStatus == UserStatusEnum.Active)
        //                    {
        //                        int timeOut = 30;
        //                        var formTicket = new FormsAuthenticationTicket(user.EmpNo, false, timeOut);
        //                        string formEncrypt = FormsAuthentication.Encrypt(formTicket);

        //                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, formEncrypt)
        //                        {
        //                            Expires = DateTime.Now.AddMinutes(timeOut),
        //                            HttpOnly = true
        //                        };

        //                        Response.Cookies.Add(cookie);

        //                        Session["UserId"] = user.EmpNo.ToLower();
        //                        Session["UserName"] = user.FullName;
        //                        Session["Designation"] = user.DesignationName;

        //                        if (user.BranchId != null)
        //                        {
        //                            Session["Branch"] = user.Branch.Name;

        //                        }
        //                        if (user.DepartmentId != null)
        //                        {
        //                            Session["Department"] = user.Department.DepartmentName;

        //                        }

        //                        if (!string.IsNullOrEmpty(user.ProImageName))
        //                        {
        //                            Session["ProfileImage"] = user.ProImageName;
        //                        }

        //                        user.LastActiveDate = DateTimeService.GetUserDate();
        //                        _service.Update(user);

        //                        Log.Info(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " - " + Session["UserId"].ToString());

        //                        NavAction(nav);
        //                        return RedirectToAction("Index", "Home");

        //                    }
        //                    else
        //                    {
        //                        NavAction(nav);
        //                        return RedirectToAction("Index", "Login");

        //                    }
        //                }
        //            }
        //        }

        //    }
        //    catch (Exception ex)
        //    {

        //        Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(), ex);

        //    }

        //    return RedirectToAction("Index", "Login");


        //}

        [HttpGet]
        public ActionResult Auth(int? nav, string key)
        {
            try
            {
                string adName = System.Web.HttpContext.Current.Request.LogonUserIdentity.Name.ToString();
                if (adName != "")
                {
                    string[] split = adName.Split('\\');
                    var userName = split[1].ToLower();

                    var user = _service.GetUserByEmpNo(userName, "Branch,Department");
                    if (user != null)
                    {
                        if (user.UserStatus == UserStatusEnum.Active)
                        {
                            int timeOut = 30;
                            var formTicket = new FormsAuthenticationTicket(user.EmpNo, false, timeOut);
                            string formEncrypt = FormsAuthentication.Encrypt(formTicket);

                            var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, formEncrypt)
                            {
                                Expires = DateTime.Now.AddMinutes(timeOut),
                                HttpOnly = true
                            };

                            Response.Cookies.Add(cookie);

                            Session["UserId"] = user.EmpNo.ToLower();
                            Session["UserName"] = user.FullName;
                            //Session["Department"] = user.DepartmentName;
                            Session["Designation"] = user.DesignationName;

                            if (user.BranchId != null)
                            {
                                Session["Branch"] = user.Branch.Name;

                            }

                            if (user.DepartmentId != null)
                            {
                                Session["Department"] = user.Department.DepartmentName;

                            }

                            if (!string.IsNullOrEmpty(user.ProImageName))
                            {
                                Session["ProfileImage"] = user.ProImageName;
                            }

                            user.LastActiveDate = DateTimeService.GetUserDate();
                            _service.Update(user);

                            Log.Info(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " - " + Session["UserId"].ToString());

                            if (nav > 0)
                            {
                                var keyText = "";
                                if (!String.IsNullOrEmpty(key) && key.All(Char.IsLetterOrDigit))
                                    keyText = key;

                                var navType = (NavAuthTypeEnum)nav;

                                if (navType == NavAuthTypeEnum.CompleteRequestedUser)
                                {
                                    return RedirectToAction("Complete", "MyTicket");
                                }
                                //else if (navType == NavAuthTypeEnum.CAUpdate)
                                //{
                                //    if (keyText != "")
                                //    {
                                //        var routeValue = GlobalStaticService.EncodedUrlParameters("id=" + GlobalStaticService.Decrypt(keyText));
                                //        return RedirectToAction("Update", "CA", new { q = routeValue });
                                //    }
                                //    else
                                //        return RedirectToAction("Index", "Home");

                                //}
                                else
                                    return RedirectToAction("Index", "Home");
                            }

                        }
                        else
                        {
                            ModelState.AddModelError("Password", "Login Failed, Please try again later!");
                        }
                    }
                }

            }

            catch (Exception ex)
            {
                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString(), ex);
            }




            return RedirectToAction("Index", "Login");


        }

        private ActionResult NavAction(int? nav, string key)
        {
            var navType = (NavAuthTypeEnum)nav;

            if (navType == NavAuthTypeEnum.CompleteRequestedUser)
            {
                return RedirectToAction("Complete", "MyTicket");
            }
            //else if (navType == NavAuthTypeEnum.CAUpdate)
            //{
            //    if (key != "")
            //    {
            //        var routeValue = GlobalStaticService.EncodedUrlParameters("id=" + GlobalStaticService.Decrypt(key));
            //        return RedirectToAction("Update", "CA", new { q = routeValue });
            //    }
            //    else
            //        return RedirectToAction("Index", "Home");

            //}

            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public ActionResult LogOut()
        {

            Session["UserId"] = null;
            Session["UserName"] = null;
            Session["ProfileImage"] = null;
            Session["Department"] = null;
            Session["Designation"] = null;
            Session["NavController"] = null;
            Session["NavAction"] = null;
            Session.Clear();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");

        }


        private void NavAction(int? nav)
        {

            if (nav == 10)//My Ticket Open 
            {
                Session["NavController"] = "MyTicket";
                Session["NavAction"] = "Open";
            }
            else if (nav == 11)//User Incident request
            {
                Session["NavController"] = "ServiceDesk";
                Session["NavAction"] = "Incident";
            }
            else if (nav == 12)//ServiceDesk/ServiceRequest
            {
                Session["NavController"] = "ServiceDesk";
                Session["NavAction"] = "ServiceRequest";
            }
            else if (nav == 15)//AccessRequest/NewRequest
            {
                Session["NavController"] = "AccessRequest";
                Session["NavAction"] = "NewRequest";
            }

        }

    }
}