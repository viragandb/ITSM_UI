using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Web.Utility
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EncryptedActionParameterAttribute : ActionFilterAttribute
    {
        private const string InitVector = "tu89geji340t89u2";

        // private const int Keysize = 256;
        private const int Keysize = 256;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            try
            {
                bool isValid = false;
                var decryptedParameters = new Dictionary<string, object>();
                if (HttpContext.Current.Request.QueryString.Get("q") != null)
                {
                    string encryptedQueryString = HttpContext.Current.Request.QueryString.Get("q");
                    string decrptedString = Decrypt(encryptedQueryString.ToString(), DateTime.Now.ToString("dd-MM-yyyy"));
                    string[] paramsArrs = decrptedString.Split('?');

                    foreach (string para in paramsArrs)
                    {
                        string[] paramArr = para.Split('=');
                        try
                        {
                            decryptedParameters.Add(paramArr[0], Convert.ToInt64(paramArr[1]));
                        }
                        catch (Exception)
                        {
                            decryptedParameters.Add(paramArr[0], paramArr[1]);

                        }

                    }
                    isValid = true;
                }
                else
                {
                    filterContext.ActionParameters.Clear();
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                    {
                        controller = "Login",
                        action = "Index",
                        area = ""
                    }));

                }


                if (isValid)
                {
                    for (int i = 0; i < decryptedParameters.Count; i++)
                    {
                        filterContext.ActionParameters[decryptedParameters.Keys.ElementAt(i)] =
                            decryptedParameters.Values.ElementAt(i);
                    }
                }
            }
            catch (Exception ex)
            {
                filterContext.ActionParameters.Clear();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new
                {
                    controller = "Login",
                    action = "Index",
                    area = ""
                }));
            }
            base.OnActionExecuting(filterContext);

        }


        public string Decrypt(string encryptedText, string key)
        {
            return GlobalStaticService.Decrypt(encryptedText, key);
        }
     
    }
}