using Domain;
using log4net;
using Newtonsoft.Json;
using Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Utility;

namespace Web.Controllers
{
    public class ItemDocController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ItemDocController));
        private readonly ItemDocService _service = new ItemDocService();
        // GET: ItemDoc
        public ActionResult Index()
        {
            return View();
        }


        [AccessLogin]
        [HttpGet]
        public ActionResult Delete(long? id)
        {
            int errorCode = 0;

            var item = _service.GetItem(id, "");
            item.UpdatedDate = UserDateTime.GetUserDate();
            item.UpdatedBy = Session["UserId"].ToString();
            if (item != null)
            {
                if (_service.HasRelationalData(item))
                    errorCode = 2;
                else if (_service.Delete(item))
                    errorCode = 1;


            }

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [AccessLogin]
        [HttpPost]
        public ActionResult UploadFiles()
        {
            try
            {
                string dirname = "UploadedFiles/";
                TicketDocService _ticketDocService = new TicketDocService();
                //string _FileName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(itemFile.FileName);
                //string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);
                string _FileName = "";
                string path = Server.MapPath("~/" + dirname);
                HttpFileCollectionBase files = Request.Files;
                int vaildFilesCount = 0;
                string errorMsg = "";
                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    _FileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    //file.FileName
                    var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                    //var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".docx", ".doc", ".xlsx", ".xls" };
                    //if (itemFile.ContentType == "application/pdf")
                    //if (file.ContentType == "application/pdf" || file.ContentType.Contains("image"))

                    if (supportedTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
                    {
                        if (file.ContentLength > 3145728) //3MB
                        {
                            errorMsg += "Please reduce file size (Max. 3MB) of " + file.FileName + "<br>";
                        }
                        else
                        {

                            file.SaveAs(path + _FileName);
                            var doc = new TempDoc();
                            doc.FileName = _FileName;
                            doc.FileUrl = "../" + dirname + _FileName;
                            doc.EmpNo = Session["UserId"].ToString();
                            doc.DocumentName = file.FileName;
                            doc.UpdatedDate = UserDateTime.GetUserDate();
                            doc.UpdatedBy = Session["UserId"].ToString();
                            _service.InsertTempDoc(doc);

                            vaildFilesCount++;
                            TempData["SuccessMessage"] = file.FileName + " file has been uploaded.";
                        }
                    }
                    else
                    {
                        errorMsg += file.FileName + " is not a valid file type " + "<br>";

                    }
                }
                return Json(errorMsg + vaildFilesCount + " Files Uploaded!");
            }
            catch (Exception ex)
            {

                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " Ticket File Upload ", ex);
                return Json(ex.Message.ToString());

            }
        }

        [AccessLogin]
        [HttpGet]
        public ActionResult DeleteTempDoc(long? id)
        {
            int errorCode = 0;
            //TicketDocService _ticketDocService = new TicketDocService();
            if (_service.DeleteTempDoc(id))
                errorCode = 1;

            return Json(JsonConvert.SerializeObject(errorCode, Formatting.None, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }), JsonRequestBehavior.AllowGet);

        }

        [HttpGet]
        [AccessLogin]
        public JsonResult GetTempDocListJsonResult()
        {
            //TicketDocService _ticketDocService = new TicketDocService();
            var items = _service.GetTempDocs(Session["UserId"].ToString(), "").ToList();

            return Json(JsonConvert.SerializeObject(items), JsonRequestBehavior.AllowGet);
        }
    }
}