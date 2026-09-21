using Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Service
{
    public static class GlobalStaticService
    {
        private const string InitVector = "tu89geji340t89u2";
        private const int Keysize = 256;

        public static string GetCurrentPage(this HtmlHelper helper, string actionName, string controllerName, string areaName)
        {
            var mvcArea = "";

            //try
            //{
            //    mvcArea = HttpContext.Current.Request.RequestContext.RouteData.DataTokens["Area"].ToString();
            //}
            //catch (NullReferenceException)
            //{
            //    mvcArea = "";
            //}

            var mvcAction = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var mvcConroller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            if ((actionName.ToUpper() == mvcAction.ToUpper() && controllerName.ToUpper() == mvcConroller.ToUpper() && areaName.ToUpper() == mvcArea.ToUpper()))
                return "active";
            else
                return "";
        }

        public static string GetCurrentMenu(this HtmlHelper helper, long menuId)
        {
            var contextOfWork = new UnitOfWork();

            var mvcArea = "";

            //try
            //{
            //    mvcArea = System.Web.HttpContext.Current.Request.RequestContext.RouteData.DataTokens["Area"].ToString();
            //}
            //catch (NullReferenceException)
            //{
            //    mvcArea = "";
            //}

            var mvcAction = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var mvcConroller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            var isMenu =
                contextOfWork.MenuItemRepository.Get(
                    e => e.MenuId == menuId && e.Area == mvcArea && e.Controller == mvcConroller).Any();

            if (isMenu)
                return "active";
            else
                return "";
        }

        public static string GetCurrentMenuOpen(this HtmlHelper helper, long menuId)
        {
            var contextOfWork = new UnitOfWork();

            var mvcArea = "";

            //try
            //{
            //    mvcArea = System.Web.HttpContext.Current.Request.RequestContext.RouteData.DataTokens["Area"].ToString();
            //}
            //catch (NullReferenceException)
            //{
            //    mvcArea = "";
            //}

            var mvcAction = HttpContext.Current.Request.RequestContext.RouteData.Values["action"].ToString();
            var mvcConroller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"].ToString();

            var isMenu =
                contextOfWork.MenuItemRepository.Get(
                    e => e.MenuId == menuId && e.Area == mvcArea && e.Controller == mvcConroller).Any();

            if (isMenu)
                return "menu-open";
            else
                return "";
        }
        public static string EnumDisplayName(this Enum enumValue)
        {
            var enumType = enumValue.GetType();
            var memberInfo = enumType.GetMember(enumValue.ToString()).First();

            if (memberInfo == null || !memberInfo.CustomAttributes.Any()) return enumValue.ToString();

            var displayAttribute = memberInfo.GetCustomAttribute<DisplayAttribute>();

            if (displayAttribute == null) return enumValue.ToString();

            if (displayAttribute.ResourceType != null && displayAttribute.Name != null)
            {
                var manager = new ResourceManager(displayAttribute.ResourceType);
                return manager.GetString(displayAttribute.Name);
            }

            return displayAttribute.Name ?? enumValue.ToString();
        }

        public static MvcHtmlString EncodedActionLink(this HtmlHelper htmlHelper, string linkText, string action, string controllerName, string areaName, object routeValues, object htmlAttributes)
        {
            string queryString = string.Empty;
            string htmlAttributesString = string.Empty;

            bool isRoute = false;
            if (routeValues != null)
            {
                var routeValueDictionary = new RouteValueDictionary(routeValues);
                for (int i = 0; i < routeValueDictionary.Keys.Count; i++)
                {
                    if (!routeValueDictionary.Keys.Contains("IsRoute"))
                    {
                        if (i > 0)
                        {
                            queryString += "?";
                        }
                        queryString += routeValueDictionary.Keys.ElementAt(i) + "=" + routeValueDictionary.Values.ElementAt(i);
                    }
                    else
                    {
                        if (!routeValueDictionary.Keys.ElementAt(i).Contains("IsRoute"))
                        {
                            queryString += routeValueDictionary.Values.ElementAt(i);
                            isRoute = true;
                        }
                    }
                }
            }

            if (htmlAttributes != null)
            {
                var routeValueDictionary = new RouteValueDictionary(htmlAttributes);
                for (int i = 0; i < routeValueDictionary.Keys.Count; i++)
                {
                    htmlAttributesString += " " + routeValueDictionary.Keys.ElementAt(i) + "=" + routeValueDictionary.Values.ElementAt(i);
                }
            }
            var ancor = new StringBuilder();

            if (!String.IsNullOrEmpty(areaName))
            {
                ancor.Append("/" + areaName + "/" + controllerName + "/" + action);
            }
            else
            {
                ancor.Append("/" + controllerName + "/" + action);
            }

            //ancor.Append("/" + action);
            if (queryString != string.Empty)
            {
                if (isRoute == false)
                    ancor.Append("?q=" + Encrypt(queryString, DateTime.Now.ToString("dd-MM-yyyy")));
                else
                    ancor.Append("?q=" + Encrypt(queryString, DateTime.Now.ToString("dd-MM-yyyy")));
            }
            //return new MvcHtmlString(ancor.ToString() + "?b=" + random);
            return new MvcHtmlString(ancor.ToString());
        }

        public static string EncodedUrlParameters(string text)
        {

            return Encrypt(text, DateTime.Now.ToString("dd-MM-yyyy"));
        }
        public static string ConvertStringToHex(String input, System.Text.Encoding encoding)
        {
            Byte[] stringBytes = encoding.GetBytes(input);
            StringBuilder sbBytes = new StringBuilder(stringBytes.Length * 2);
            foreach (byte b in stringBytes)
            {
                sbBytes.AppendFormat("{0:X2}", b);
            }
            return sbBytes.ToString();
        }

        public static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[maxSize];
                crypto.GetNonZeroBytes(data);
            }
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
        public static string Encrypt(string text, string key)
        {
            byte[] initVectorBytes = Encoding.UTF8.GetBytes(InitVector);
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(text);
            PasswordDeriveBytes password = new PasswordDeriveBytes(key, null);
            byte[] keyBytes = password.GetBytes(Keysize / 8);
            RijndaelManaged symmetricKey = new RijndaelManaged();
            symmetricKey.Mode = CipherMode.CBC;
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            MemoryStream memoryStream = new MemoryStream();
            CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
            cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
            cryptoStream.FlushFinalBlock();
            byte[] Encrypted = memoryStream.ToArray();
            memoryStream.Close();
            cryptoStream.Close();
            return ConvertStringToHex(Convert.ToBase64String(Encrypted), System.Text.Encoding.Unicode);//Convert.ToBase64String(Encrypted);
        }

        public static string Decrypt(string encryptedText, string key)
        {
            try
            {
                byte[] initVectorBytes = Encoding.ASCII.GetBytes(InitVector);
                // byte[] DeEncryptedText = Convert.FromBase64String(ConvertHexToString(encryptedText.Split('?')[0], Encoding.Unicode));
                byte[] deEncryptedText = Convert.FromBase64String(ConvertHexToString(encryptedText, Encoding.Unicode));
                PasswordDeriveBytes password = new PasswordDeriveBytes(key, null);
                byte[] keyBytes = password.GetBytes(Keysize / 8);
                RijndaelManaged symmetricKey = new RijndaelManaged();
                symmetricKey.Mode = CipherMode.CBC;
                ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
                MemoryStream memoryStream = new MemoryStream(deEncryptedText);
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
                byte[] plainTextBytes = new byte[deEncryptedText.Length];
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                memoryStream.Close();
                cryptoStream.Close();
                return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch (Exception ex)
            {
                return "";
            }
        }

        public static string Encrypt(string text)
        {
            return Encrypt(text, InitVector);
        }
        public static string Decrypt(string text)
        {
            return Decrypt(text, InitVector);
        }

        public static string ConvertHexToString(String hexInput, Encoding encoding)
        {
            try
            {
                int numberChars = hexInput.Length;
                byte[] bytes = new byte[numberChars / 2];
                for (int i = 0; i < numberChars; i += 2)
                {
                    bytes[i / 2] = Convert.ToByte(hexInput.Substring(i, 2), 16);
                }
                return encoding.GetString(bytes);
            }
            catch (Exception)
            {
                return "";
            }

        }

        #region Form Encryption
        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, Object obj)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, RouteValueDictionary routeValueDictionary)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, Object obj)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, FormMethod formMethod)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, RouteValueDictionary routeValueDictionary)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, Object obj, FormMethod formMethod)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, FormMethod formMethod, IDictionary<String, Object> iDictionary)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, FormMethod formMethod, Object obj)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }
        //public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, FormMethod formMethod, Object obj,Object obje)
        //{
        //    var mvcForm = htmlHelper.BeginForm();
        //    htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
        //    return mvcForm;
        //}
        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, RouteValueDictionary routeValueDictionary, FormMethod formMethod)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, Object obj, FormMethod formMethod, Object htmlAttribute)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }

        public static MvcForm EncryptBeginForm(this HtmlHelper htmlHelper, String action, String controller, RouteValueDictionary routeValueDictionary, FormMethod formMethod,
            IDictionary<String, Object> iDictionary)
        {
            var mvcForm = htmlHelper.BeginForm();
            htmlHelper.ViewContext.Writer.Write(htmlHelper.AntiForgeryToken().ToHtmlString());
            return mvcForm;
        }
        #endregion

        public static List<SelectListItem> GetIcons()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "check-square-o", Value = "check-square-o" });
            items.Add(new SelectListItem { Text = "pencil-square-o", Value = "pencil-square-o" });
            items.Add(new SelectListItem { Text = "tags", Value = "tags" });
            items.Add(new SelectListItem { Text = "wrench", Value = "wrench" });
            items.Add(new SelectListItem { Text = "user-o", Value = "user-o" });
            items.Add(new SelectListItem { Text = "window-maximize", Value = "window-maximize" });
            items.Add(new SelectListItem { Text = "bars", Value = "bars" });
            items.Add(new SelectListItem { Text = "bell", Value = "bell" });
            items.Add(new SelectListItem { Text = "briefcase", Value = "briefcase" });
            items.Add(new SelectListItem { Text = "book", Value = "book" });
            items.Add(new SelectListItem { Text = "bullhorn", Value = "bullhorn" });
            items.Add(new SelectListItem { Text = "calendar-o", Value = "calendar-o" });
            items.Add(new SelectListItem { Text = "database", Value = "database" });
            items.Add(new SelectListItem { Text = "envelope", Value = "envelope" });
            items.Add(new SelectListItem { Text = "folder-open", Value = "folder-open" });
            items.Add(new SelectListItem { Text = "gavel", Value = "gavel" });
            items.Add(new SelectListItem { Text = "phone", Value = "phone" });
            items.Add(new SelectListItem { Text = "sticky-note", Value = "sticky-note" });
            items.Add(new SelectListItem { Text = "volume-up", Value = "volume-up" });
            items.Add(new SelectListItem { Text = "wifi", Value = "wifi" });
            items.Add(new SelectListItem { Text = "bug", Value = "bug" });
            items.Add(new SelectListItem { Text = "picture-o", Value = "picture-o" });
            items.Add(new SelectListItem { Text = "mobile", Value = "mobile" });
            items.Add(new SelectListItem { Text = "print", Value = "print" });
            items.Add(new SelectListItem { Text = "upload", Value = "upload" });
            items.Add(new SelectListItem { Text = "plug", Value = "plug" });
            items.Add(new SelectListItem { Text = "desktop", Value = "desktop" });
            items.Add(new SelectListItem { Text = "lock", Value = "lock" });
            items.Add(new SelectListItem { Text = "chrome", Value = "chrome" });
            items.Add(new SelectListItem { Text = "laptop", Value = "laptop" });


            return items;
        }

        public static List<SelectListItem> GetColors()
        {
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "default", Value = "default" });
            items.Add(new SelectListItem { Text = "primary", Value = "primary" });
            items.Add(new SelectListItem { Text = "info", Value = "info" });
            items.Add(new SelectListItem { Text = "success", Value = "success" });
            items.Add(new SelectListItem { Text = "danger", Value = "danger" });
            items.Add(new SelectListItem { Text = "warning", Value = "warning" });

            return items;
        }

        public static string GetSystemUser()
        {
            return "91000";
        }
        //public static SelectList GetEnumSelectList<TEnum>(TEnum value)
        // {
        //     //Array values = Enum.GetValues(value);
        //     //List<ListItem> items = new List<ListItem>(values.Length);

        //     //foreach (var i in values)
        //     //{
        //     //    items.Add(new ListItem
        //     //    {
        //     //        Text = Enum.GetName(typeof(ProjectStatusEnum), i),
        //     //        Value = i.ToString()
        //     //    });
        //     //}


        //     IEnumerable<TEnum> values = Enum.GetValues(value.GetType()).Cast<TEnum>();

        //     IEnumerable<SelectListItem> items = from v in values
        //                                         select new SelectListItem
        //                                         {
        //                                             Text = GetEnumDescription(value),
        //                                             Value = value.ToString()

        //                                         };

        //     return new SelectList(items);
        // }
        //private static Type GetNonNullableModelType(ModelMetadata modelMetadata)
        //{
        //    Type realModelType = modelMetadata.ModelType;

        //    Type underlyingType = Nullable.GetUnderlyingType(realModelType);
        //    if (underlyingType != null)
        //    {
        //        realModelType = underlyingType;
        //    }
        //    return realModelType;
        //}

        //private static readonly SelectListItem[] SingleEmptyItem = new[] { new SelectListItem { Text = "", Value = "" } };

        //public static string GetEnumDescription<TEnum>(TEnum value)
        //{
        //    FieldInfo fi = value.GetType().GetField(value.ToString());

        //    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

        //    if ((attributes != null) && (attributes.Length > 0))
        //        return attributes[0].Description;
        //    else
        //        return value.ToString();
        //}

        //public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression)
        //{
        //    return EnumDropDownListFor(htmlHelper, expression, null);
        //}

        //public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TEnum>> expression, object htmlAttributes)
        //{
        //    ModelMetadata metadata = ModelMetadata.FromLambdaExpression(expression, htmlHelper.ViewData);
        //    Type enumType = GetNonNullableModelType(metadata);
        //    IEnumerable<TEnum> values = Enum.GetValues(enumType).Cast<TEnum>();

        //    IEnumerable<SelectListItem> items = from value in values
        //                                        select new SelectListItem
        //                                        {
        //                                            Text = GetEnumDescription(value),
        //                                            Value = value.ToString(),
        //                                            Selected = value.Equals(metadata.Model)
        //                                        };

        //    // If the enum is nullable, add an 'empty' item to the collection
        //    if (metadata.IsNullableValueType)
        //        items = SingleEmptyItem.Concat(items);

        //    return htmlHelper.DropDownListFor(expression, items, htmlAttributes);
        //}

        public static string GetSystemName()
        {
            return "IT Service Management System";
        }

        public static string GetSystemNameCode()
        {
            return "ITSM";
        }

        public static string GetAppUrl()
        {
            //return ConfigurationManager.AppSettings["AppUrl"];
            return "https://itsm.ndblk.int";
        }

        public static string GetFileUploadPath()
        {
            return ConfigurationManager.AppSettings["FileUploadPath"]; ;
        }

        public static DateTime GetUserDate()
        {
            //return DateTime.UtcNow;

            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.Now, "Sri Lanka Standard Time");

        }

        public static DateTime ConvertToAppDateTime(DateTime dateTime)
        {
            //return DateTime.UtcNow;
            CultureInfo culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            culture.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            //culture.DateTimeFormat.LongTimePattern = "";
            Thread.CurrentThread.CurrentCulture = culture;

            return dateTime;

        }

        public static string ConvertHHmm(double hrs)
        {
            hrs = hrs * 60;
            int n = Convert.ToInt32(hrs);
            int hour = n / 60;
            n %= 60;
            int minutes = n;
            return hour.ToString("00") + ":" + minutes.ToString("00");
        }

        public static int GetMaximumFileSize()
        {
            return 3145728; //3MB
        }

    }
}
