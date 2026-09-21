using Domain;
using log4net;
using Newtonsoft.Json;
using OfficeOpenXml;
using Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Models;
using Web.Utility;

namespace Web.Controllers
{
    public class AssetVerificationController : Controller
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(AssetVerificationController));
        private readonly AssetVerificationRequestService _service = new AssetVerificationRequestService();
        private readonly AssetService _assetService = new AssetService();

        [AccessAuthorize]
        public ActionResult Index(long? assetCategoryId, long? branchId, long? departmentId, string startDate, string endDate)
        {
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;
            if (assetCategoryId == null || assetCategoryId == 0)
                assetCategoryId = (long)AssetCategoryEnum.ITAsset;
            TicketService _ticketService = new TicketService();
            DateTime sDate = UserDateTime.GetUserDateOnly();
            DateTime eDate = UserDateTime.GetUserDateOnly();
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = UserDateTime.ConvertToAppDateTime(startDate);
                    eDate = UserDateTime.ConvertToAppDateTime(endDate);
                }
                catch (Exception ex) { }
            }

            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AssetCategoryId = assetCategoryId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            var items = _service.GetItems(assetCategoryId, branchId, departmentId, sDate, eDate.AddDays(1),
                "Branch,Department,RequestedUser,Assets").OrderByDescending(o => o.RequestedDate).ToList();
            return View(items);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Create(long? assetCategoryId, long? branchId, long? departmentId)
        {
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;
            if (assetCategoryId == null || assetCategoryId == 0)
                assetCategoryId = (long)AssetCategoryEnum.ITAsset;

            var item = new AssetVerificationVM();
            item.VerificationDate = UserDateTime.GetUserDateOnly().AddDays(1).ToString("dd/MM/yyyy");

            item.Assets = _assetService.GetItemsAvbByLocation(assetCategoryId, 0, branchId, departmentId, "AssetType,AssetMake").ToList();
            item.BranchId = (long)branchId;
            item.DepartmentId = (long)departmentId;
            item.AssetCategory = (AssetCategoryEnum)assetCategoryId;
            ViewBag.VerificationDate = item.VerificationDate;
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AssetCategoryId = assetCategoryId;
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Create(AssetVerificationVM item)
        {
            var verificationDate = UserDateTime.GetUserDate();
            ViewBag.VerificationDate = item.VerificationDate;
            ViewBag.BranchId = item.BranchId;
            ViewBag.DepartmentId = item.DepartmentId;
            ViewBag.AssetCategoryId = item.AssetCategory;
            item.Assets = _assetService.GetItemsAvbByLocation((long)item.AssetCategory, 0, item.BranchId, item.DepartmentId, "AssetType,AssetMake").ToList();

            if (item.BranchId == 0 || item.DepartmentId == 0 || item.AssetCategory == 0)
            {
                TempData["ErrorMessage"] = "Please enter required data. ";
                return View("Create", item);
            }

            try
            {
                verificationDate = UserDateTime.ConvertToAppDateTime(item.VerificationDate);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Please enter valid verification date. ";
                return View(item);
            }

            if (verificationDate < UserDateTime.GetUserDateOnly())
            {
                TempData["ErrorMessage"] = "Please enter valid verification date. ";
                return View(item);
            }

            AssetVerificationRequest request = new AssetVerificationRequest();
            request.UpdatedBy = Session["UserId"].ToString();
            request.UpdatedDate = UserDateTime.GetUserDate();
            request.RequestedBy = request.UpdatedBy;
            request.RequestedDate = request.UpdatedDate;
            request.BranchId = item.BranchId;
            request.DepartmentId = item.DepartmentId;
            request.AssetCategory = item.AssetCategory;
            request.VerificationDate = verificationDate;
            request.VerifiedDate = verificationDate;
            request.Status = AssetVerificationStatusEnum.Pending;
            if (item.Assets != null)
            {
                Collection<AssetVerificationItem> assets = new Collection<AssetVerificationItem>();
                foreach (var _asset in item.Assets)
                {
                    AssetVerificationItem asset = new AssetVerificationItem();
                    asset.AssetId = _asset.AssetId;
                    asset.UpdatedBy = request.UpdatedBy;
                    asset.UpdatedDate = request.UpdatedDate;
                    assets.Add(asset);

                }
                request.Assets = assets;
            }
            else
            {
                TempData["ErrorMessage"] = "Invalid request.";
                return View(item);
            }

            var entitySaved = _service.Insert(request);
            if (entitySaved)
            {

                TempData["SuccessMessage"] = "Request has been successfully submited.";
                return RedirectToAction("Create");
            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View("Create", item);
            }

        }

        [HttpGet]
        [AccessLogin]
        [EncryptedActionParameter]
        public ActionResult Info(long? id)
        {
            if (id == null)
                return HttpNotFound();
            var item = _service.GetItem(id, "Branch,Department,RequestedUser,Assets,VerifiedUser,Assets.Asset.AssetType,Assets.Asset.AssetMake");

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult PendingToBeCreated(long? assetCategoryId, long? branchId, string startDate, string endDate)
        {
            if (branchId == null)
                branchId = 0;
           
            if (assetCategoryId == null || assetCategoryId == 0)
                assetCategoryId = (long)AssetCategoryEnum.ITAsset;

            TicketService _ticketService = new TicketService();
            DateTime sDate = UserDateTime.GetUserDateOnly().AddMonths(-1);
            DateTime eDate = UserDateTime.GetUserDateOnly().AddDays(1);
            if (startDate != null && startDate != "" && endDate != null && endDate != "")
            {
                try
                {
                    sDate = UserDateTime.ConvertToAppDateTime(startDate);
                    eDate = UserDateTime.ConvertToAppDateTime(endDate);
                }
                catch (Exception ex) { }
            }

            ViewBag.BranchId = branchId;
            ViewBag.AssetCategoryId = assetCategoryId;
            ViewBag.StartDate = sDate.ToString("dd/MM/yyyy");
            ViewBag.EndDate = eDate.ToString("dd/MM/yyyy");

            BranchService _branchService = new BranchService();

            Collection<AVPendingBranchVM> items = new Collection<AVPendingBranchVM>();

            if(branchId==0)
            {
                var branches = _branchService.GetAll().OrderBy(o=>o.Name);
                foreach(Branch b in branches)
                {
                    AVPendingBranchVM item = new AVPendingBranchVM();
                    item.BranchId = b.BranchId;
                    item.BranchName = b.Name;
                    items.Add(item);
                }
            }
            else
            {
                Branch b = _branchService.GetAsNoTrackingItem(branchId, "");
                AVPendingBranchVM item = new AVPendingBranchVM();
                item.BranchId = b.BranchId;
                item.BranchName = b.Name;
                items.Add(item);
            }

            DepartmentService _deptService = new DepartmentService();
            foreach(AVPendingBranchVM item in items)
            {
                Collection<AVPendingDeptVM> depts = new Collection<AVPendingDeptVM>();
                var _depts = _deptService.GetItemsByBranchId(item.BranchId, "");

                foreach (Department d in _depts)
                {
                    AVPendingDeptVM _d = new AVPendingDeptVM();
                    _d.DepartmentId = d.DepartmentId;
                    _d.Departmentname = d.DepartmentName;

                    var requests = _service.GetItems(assetCategoryId, item.BranchId, 0, sDate, eDate, "");
                    var request = requests.Where(w => w.DepartmentId == d.DepartmentId).OrderBy(o => o.UpdatedDate).FirstOrDefault();
                    if(request!=null)
                    {
                        _d.Status = request.Status.EnumDisplayName();
                        _d.Date = request.VerificationDate.ToShortDateString();
                        _d.AssetVerificationId = request.AssetVerificationRequestId;
                    }
                    else
                    {
                        _d.Status = "Pending";
                        _d.Date = "N/A";
                    }
                    depts.Add(_d);
                }
                item.PendingDepartments = depts;
            }

            //var items = _service.GetItems(assetCategoryId, branchId, departmentId, sDate, eDate.AddDays(1),
            //    "Branch,Department,RequestedUser,Assets").OrderByDescending(o => o.RequestedDate).ToList();
            return View(items);

        }

        [AccessAuthorize]
        [HttpGet]
        public ActionResult Pending(long? assetCategoryId, long? branchId, long? departmentId)
        {
            if (branchId == null)
                branchId = 0;
            if (departmentId == null)
                departmentId = 0;
            if (assetCategoryId == null || assetCategoryId == 0)
                assetCategoryId = (long)AssetCategoryEnum.ITAsset;

            var items = _service.GetItemsPending(assetCategoryId, branchId, departmentId, "Branch,Department,RequestedUser,Assets");
            ViewBag.BranchId = branchId;
            ViewBag.DepartmentId = departmentId;
            ViewBag.AssetCategoryId = assetCategoryId;
            return View(items);
        }

        public ActionResult DownloadExcel(long? id)
        {
            //var excelContent = @"
            //    <table border='1'>
            //        <tr>
            //            <th>Name</th>
            //            <th>Age</th>
            //            <th>Country</th>
            //        </tr>
            //        <tr>
            //            <td>John Doe</td>
            //            <td>30</td>
            //            <td>USA</td>
            //        </tr>
            //        <tr>
            //            <td>Jane Smith</td>
            //            <td>25</td>
            //            <td>Canada</td>
            //        </tr>
            //    </table>";

            //// Set response headers
            //Response.Clear();
            ////Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            //Response.ContentType = "application/vnd.ms-excel";
            //Response.AddHeader("Content-Disposition", "attachment; filename=example.xls");
            //var byteArray = Encoding.UTF8.GetBytes(excelContent);
            //var stream = new MemoryStream(byteArray);
            //return new FileStreamResult(stream, "application/vnd.ms-excel");

            //DataSet dataSet = GetSampleDataSet();

            var request = _service.GetItemAsNoTracking(id, "Branch,Department,Assets,Assets.Asset,Assets.Asset.AssetType,Assets.Asset.AssetMake");
            DataSet dataSet = new DataSet();

            // Create a DataTable
            DataTable table = new DataTable();

            // Add columns to the DataTable
            table.Columns.Add("Branch", typeof(string));
            table.Columns.Add("Department", typeof(string));
            table.Columns.Add("AssetNo", typeof(string));
            table.Columns.Add("Barcode", typeof(string));
            table.Columns.Add("SerialNo", typeof(string));
            table.Columns.Add("AssetType", typeof(string));
            table.Columns.Add("AssetMake", typeof(string));
            table.Columns.Add("ModelName", typeof(string));
            table.Columns.Add("Availability(Y/N)", typeof(string));
            table.Columns.Add("Comment", typeof(string));

            foreach (AssetVerificationItem asset in request.Assets)
            {
                DataRow dr = table.NewRow();
                dr[0] = request.Branch.Name;
                dr[1] = request.Department.DepartmentName;
                dr[2] = asset.Asset.AssetNo;
                dr[3] = asset.Asset.Barcode;
                dr[4] = asset.Asset.SerialNo;
                dr[5] = asset.Asset.AssetType.AssetTypeName;
                dr[6] = asset.Asset.AssetMake.Name;
                dr[7] = asset.Asset.ModelName;
                dr[8] = "";
                dr[9] = "";

                table.Rows.Add(dr);
            }
            DataRow dr4 = table.NewRow();
            dr4[0] = "";
            dr4[1] = "";
            dr4[2] = "";
            dr4[3] = "";
            dr4[4] = "";
            dr4[5] = "";
            dr4[6] = "";
            dr4[7] = "";
            dr4[8] = "";
            dr4[9] = "";
            table.Rows.Add(dr4);
            DataRow dr2 = table.NewRow();
            dr2[0] = "Verified By :";
            dr2[2] = "Name";
            dr2[3] = ".......................................";
            dr2[4] = "";
            dr2[5] = "Sign";
            dr2[6] = ".......................................";
            dr2[7] = "";
            dr2[8] = "Date";
            dr2[9] = ".......................................";
            dr2[7] = "";
            table.Rows.Add(dr2);
            DataRow dr6 = table.NewRow();
            dr6[0] = "";
            dr6[1] = "";
            dr6[2] = "";
            dr6[3] = "";
            dr6[4] = "";
            dr6[5] = "";
            dr6[6] = "";
            dr6[7] = "";
            dr6[8] = "";
            dr6[9] = "";
            table.Rows.Add(dr6);
            DataRow dr3 = table.NewRow();
            dr3[0] = "Approved By ";
            dr3[1] = " ";
            dr3[2] = "Name :";
            dr3[3] = ".......................................";
            dr3[4] = "";
            dr3[5] = "Sign";
            dr3[6] = ".......................................";
            dr3[7] = "";
            dr3[8] = "Date";
            dr3[9] = ".......................................";
            table.Rows.Add(dr3);
            string fileName = request.RequestedDate.Year + "_" + request.Branch.Name.Replace(" ","_") + "_" + request.Department.DepartmentName.Replace(" ", "_") + "_" + "AssetList";


            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet1");

                // Add some dummy data
                worksheet.Cells["A1"].Value = "Hello";
                ////worksheet.Cells["B1"].Value = "World";
                worksheet.Cells["A1"].LoadFromDataTable(table, true);
                MemoryStream stream = new MemoryStream();
                excelPackage.SaveAs(stream);
                stream.Position = 0;

                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName + ".xlsx");
            }


            //var workbook = new XLWorkbook();
            //var worksheet = workbook.Worksheets.Add("Sheet1");

            //// Add some dummy data
            //worksheet.Cell("A1").Value = "Hello";
            //worksheet.Cell("B1").Value = "World";

            //// Save the workbook to a memory stream
            //MemoryStream stream = new MemoryStream();
            //workbook.SaveAs(stream);

            //// Set the position of the stream to 0 to ensure proper reading
            //stream.Position = 0;

            //// Return the Excel file as a file download
            //return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "example.xlsx");


        }

        [HttpGet]
        [AccessAuthorize]
        [EncryptedActionParameter]
        public ActionResult Verify(long? id)
        {

            if (id == null)
                return HttpNotFound();

            var item = new AssetVerifyVM();
            item.Request = _service.GetItemAsNoTracking(id, "Branch,Department,RequestedUser,Assets");
            item.RequestId = item.Request.AssetVerificationRequestId;
            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [AccessAuthorize]
        [HttpPost]
        public ActionResult Verify(AssetVerifyVM item, HttpPostedFileBase fileInputVerifiedDoc)
        {
            var request = _service.GetItemAsNoTracking(item.RequestId, "Branch,Department");

            item.Request = request;
            if(item.Assets==null)
            {
                TempData["ErrorMessage"] = "Please upload asset list.";
                return View(item);
            }
            foreach (AssetVM asset in item.Assets)
            {
                if ((asset.Comment == "" || asset.Comment == null) && (asset.Availability != "Y" && asset.Availability != "y"))
                {
                    TempData["ErrorMessage"] = "Please update " + asset.AssetNo + " status";
                    return View(item);
                }
            }

            if (fileInputVerifiedDoc == null || fileInputVerifiedDoc.ContentLength == 0)
            {
                TempData["ErrorMessage"] = "Please add valid verified doc. ";
                return View(item);
            }

            else if (fileInputVerifiedDoc.ContentLength > GlobalStaticService.GetMaximumFileSize()) //3MB
            {
                TempData["ErrorMessage"] = "Please reduce file size (Max. 3MB). ";
                return View(item);
            }

            var supportedTypes = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
            if (!supportedTypes.Contains(Path.GetExtension(fileInputVerifiedDoc.FileName).ToLower()))
            {
                TempData["ErrorMessage"] = "Please upload a valid format. ";
                return View(item);
            }

            foreach (AssetVM asset in item.Assets)
            {
                var assetItem = _service.GetAssetVerificationItem(request.AssetVerificationRequestId, asset.AssetNo, "");
                if (asset.Availability == "Y" || asset.Availability == "y")
                    assetItem.IsAvailable = true;
                else
                    assetItem.IsAvailable = false;

                assetItem.Comment = asset.Comment;
                assetItem.UpdatedBy = Session["UserId"].ToString();
                assetItem.UpdatedDate = UserDateTime.GetUserDate();
                _service.UpdateAssetVerificationItem(assetItem);

                var _asset = _assetService.GetItem(asset.AssetId, "");
                _asset.VerifiedBy = assetItem.UpdatedBy;
                _asset.VerifiedDate = assetItem.UpdatedDate;
                _asset.UpdatedBy = assetItem.UpdatedBy;
                _asset.UpdatedDate = assetItem.UpdatedDate;
                _assetService.Update(_asset);

            }

            var updateRequest = _service.GetItem(request.AssetVerificationRequestId, "");
            updateRequest.UpdatedBy = Session["UserId"].ToString();
            updateRequest.UpdatedDate = UserDateTime.GetUserDate();
            updateRequest.Comment = item.FinalComment;
            updateRequest.Status = AssetVerificationStatusEnum.Completed;
            updateRequest.VerifiedBy = updateRequest.UpdatedBy;
            updateRequest.VerifiedDate = updateRequest.UpdatedDate;

            string dirname = GlobalStaticService.GetFileUploadPath();
            string _FileName = Guid.NewGuid().ToString() + Path.GetExtension(fileInputVerifiedDoc.FileName);
            string _path = Path.Combine(Server.MapPath("~/" + dirname), _FileName);

            fileInputVerifiedDoc.SaveAs(_path);
            updateRequest.VerifiedDocName = "/" + dirname + _FileName;


            var entitySaved = _service.Update(updateRequest);
            if (entitySaved)
            {
                foreach (AssetVM asset in item.Assets)
                {
                    var _asset = _assetService.GetItem(asset.AssetId, "");
                    _asset.VerifiedBy = updateRequest.UpdatedBy;
                    _asset.VerifiedDate = updateRequest.UpdatedDate;
                    _asset.UpdatedBy = updateRequest.UpdatedBy;
                    _asset.UpdatedDate = updateRequest.UpdatedDate;
                    _assetService.Update(_asset);

                }
                TempData["SuccessMessage"] = "Request has been successfully updated.";
                return RedirectToAction("Pending");
            }
            else
            {
                TempData["ErrorMessage"] = "Record couldn't save, Please contact the IT support.";
                return View(item);
            }

        }

        [AccessLogin]
        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase file, string additionalParam)
        {
            try
            {
                string dirname = GlobalStaticService.GetFileUploadPath();
                //TicketDocService _ticketDocService = new TicketDocService();
                //ItemDocService _docService = new ItemDocService();
                string _FileName = "";
                string path = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());

                HttpFileCollectionBase files = Request.Files;
                int vaildFilesCount = 0;
                string errorMsg = "";
                //for (int i = 0; i < files.Count; i++)
                {
                    //HttpPostedFileBase file = files[i];
                    _FileName = Guid.NewGuid().ToString() + "-" + Path.GetExtension(file.FileName);
                    var supportedTypes = new[] { ".xlsx", ".xls" };

                    if (supportedTypes.Contains(Path.GetExtension(file.FileName).ToLower()))
                    {
                        if (file.ContentLength > GlobalStaticService.GetMaximumFileSize()) //3MB
                        {
                            errorMsg += "Please reduce file size (Max. 3MB) of " + file.FileName + "<br>";
                        }
                        else
                        {

                            file.SaveAs(path + _FileName);
                            UpdateAssetList(path + _FileName, additionalParam);
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

                Log.Error(RouteData.Values["controller"].ToString() + "/" + RouteData.Values["action"].ToString() + " File Upload ", ex);
                return Json(ex.Message.ToString());

            }
        }

        private void UpdateAssetList(string fileName, string requestid)
        {
            long assetVerificationId = 0;
            try
            {
                assetVerificationId = Convert.ToInt64(requestid);
            }
            catch (Exception ex) { }
            //string path = Server.MapPath("~/" + GlobalStaticService.GetFileUploadPath());
            //string _FileName = path + docName;

            // var fileName = "D:\\Project_Doc\\ITSM\\Master Data\\" + docName;
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileName + ";Extended Properties=Excel 12.0");
            //OleDbConnection connection = new OleDbConnection("Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileName + ";Extended Properties=Excel 8.0");

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            // string firstSheetName = dtSchema.Rows[0]["TABLE_NAME"].ToString();

            string query = "select * from [Sheet1$]";
            OleDbDataAdapter da = new OleDbDataAdapter(query, connection);
            DataSet ds = new DataSet();
            da.Fill(ds);
            DataTable dt = new DataTable();
            dt = ds.Tables[0];
            connection.Close();

            StringBuilder sqlSrt = new StringBuilder();
            long rowIndex = 0;

            SerialService _serialService = new SerialService();

            foreach (DataRow r in dt.Rows)
            {
                var _asset = new Asset();
                rowIndex++;
                string assetNo = r[2].ToString().Trim();
                string avb = r[8].ToString().Trim();
                string comment = r[9].ToString().Trim();
                if (assetNo != "")
                {
                    try
                    {
                        var assetItem = _service.GetAssetVerificationItem(assetVerificationId, assetNo, "");
                        if (avb == "Y" || avb == "y")
                            assetItem.IsAvailable = true;
                        assetItem.Comment = comment;
                        assetItem.UpdatedBy = Session["UserId"].ToString();
                        assetItem.UpdatedDate = UserDateTime.GetUserDate();

                        _service.UpdateAssetVerificationItem(assetItem);
                    }
                    catch (Exception ex)
                    {

                    }
                }

            }
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetAssetListJsonResult(long? requestId)
        {

            var request = _service.GetItemAsNoTracking(requestId, "Branch,Department,Assets,Assets.Asset,Assets.Asset.AssetType,Assets.Asset.AssetMake");
            Collection<AssetVM> assets = new Collection<AssetVM>();
            foreach (AssetVerificationItem asset in request.Assets)
            {
                AssetVM a = new AssetVM();
                a.AssetId = asset.Asset.AssetId;
                a.AssetNo = asset.Asset.AssetNo;
                a.Barcode = asset.Asset.Barcode;
                a.SerialNo = asset.Asset.SerialNo;
                a.AssetTypeName = asset.Asset.AssetType.AssetTypeName;
                a.AssetMakeName = asset.Asset.AssetMake.Name;
                a.ModelName = asset.Asset.ModelName;
                if (asset.IsAvailable)
                    a.Availability = "Y";
                else
                    a.Availability = "N";

                a.Comment = asset.Comment;

                assets.Add(a);
            }

            return Json(JsonConvert.SerializeObject(assets), JsonRequestBehavior.AllowGet);
        }
        private DataSet GetSampleDataSet()
        {
            // Create a new DataSet
            DataSet dataSet = new DataSet("SampleDataSet");

            // Create a DataTable
            DataTable table = new DataTable("SampleTable");

            // Add columns to the DataTable
            table.Columns.Add("ID", typeof(int));
            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Age", typeof(int));

            // Add rows to the DataTable
            table.Rows.Add(1, "John", 30);
            table.Rows.Add(2, "Jane", 25);

            // Add the DataTable to the DataSet
            dataSet.Tables.Add(table);

            return dataSet;
        }
        
        private string GenerateExcelContent(DataSet dataSet)
        {
            StringBuilder sb = new StringBuilder();

            // Add headers
            foreach (DataColumn column in dataSet.Tables[0].Columns)
            {
                sb.Append(column.ColumnName + "\t");
            }
            sb.Append("\n");

            // Add data rows
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    sb.Append(item.ToString() + "\t");
                }
                sb.Append("\n");
            }

            return sb.ToString();
        }
    
    }
}