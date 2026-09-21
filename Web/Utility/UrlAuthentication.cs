using Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Utility
{
    public class UrlAuthentication
    {

        public static string CreateAuthUrlRate(string id, DateTime date)
        {
            return CreateAuthUrl(GetUrlRateType(), date.AddDays(3), id);
        }

        public static string CreateAuthUrlChangeApprove(string id, DateTime date)
        {
            return CreateAuthUrl(GetUrlChangeApprove(), date.AddDays(3), id);
        }

        public static string CreateAuthUrlReviewTask(string id, DateTime date)
        {
            return CreateAuthUrl(GetUrlReviewTask(), date.AddDays(3), id);
        }

        public static string CreateAuthUrl(string type, DateTime date, string id)
        {

            string queryString = "AT=" + type + "?AD=" + date.ToString("MM/dd/yyyy") + "?ID=" + id + "";

            return GlobalStaticService.Encrypt(queryString);
        }
        public static string CreateAuthUrlByUserId(string userId, string type, DateTime date, string id)
        {

            string queryString = "UID=" + userId + "?AT=" + type + "?AD=" + date.ToString("MM/dd/yyyy") + "?ID=" + id + "";

            return GlobalStaticService.Encrypt(queryString);
        }
        public static string GetUrlRateType()
        {
            return "R";

        }

        public static string GetUrlChangeApprove()
        {
            return "CA";

        }
        public static string GetUrlReviewTask()
        {
            return "STR";

        }
    }
}