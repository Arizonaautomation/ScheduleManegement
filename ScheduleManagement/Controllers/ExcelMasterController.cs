using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.Models;
using TrainningManagement.SessionValidate;

namespace TrainningManagement.Controllers
{
    #region
    [ActionAuthorize]
    #endregion
    public class ExcelMasterController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();

        // GET: ExcelMaster
        public ActionResult Index()
        {
            ViewBag.FileList = filelist();
            ViewBag.ChildFileList = Childfilelist();
            ViewBag.Mlist = masterList();
            return View();
        }

        public List<FileUploadModel> filelist()
        {
            List<FileUploadModel> List = new List<FileUploadModel>();
            string path = Server.MapPath("~/Content/UploadedFolder");
            string[] filePaths = Directory.GetFiles(path, "*.xls");
            FileUploadModel file;
            foreach (var item in filePaths)
            {
                file = new FileUploadModel();
                file.file = Path.GetFileName(item);
                // file.file = item;
                List.Add(file);
            }
            return List;
        }

        public List<FileUploadModel> Childfilelist()
        {
            List<FileUploadModel> List = new List<FileUploadModel>();
            string path = Server.MapPath("~/Content/UploadedFolder/File");
            string[] filePaths = Directory.GetFiles(path, "*.xls");
            FileUploadModel file;
            foreach (var item in filePaths)
            {
                file = new FileUploadModel();
                file.file = Path.GetFileName(item);
                // file.file = item;
                List.Add(file);
            }
            return List;
        }

        //public static string Base64Decode(string base64EncodedData)
        //{
        //    var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
        //    return Encoding.UTF8.GetString(base64EncodedBytes);
        //}

        public List<tblExcelMaster> masterList()
        {
            List<tblExcelMaster> List = new List<tblExcelMaster>();
            List = scheModel.tblExcelMasters.ToList();
            //tblExcelMaster data = new tblExcelMaster();
            //foreach (var item in Data)
            //{
            //    data.MasterExcel_Id = item.MasterExcel_Id;
            //    data.FileName = Base64Decode(item.FileName);
            //    List.Add(data);
            //}
            return List;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {

                        if (file != null)
                        {
                            string path = Path.Combine(Server.MapPath("~/Content/UploadedFolder"), Path.GetFileName(file.FileName));
                            file.SaveAs(path);

                        }
                        ViewBag.FileStatus = "File uploaded successfully.";
                    }
                    catch (Exception)
                    {

                        ViewBag.FileStatus = "Error while file uploading.";
                    }

                }
                ViewBag.FileList = filelist();
                ViewBag.ChildFileList = Childfilelist();
                return View("Index");


                //string filePath = string.Format("{0}/{1}", Server.MapPath("~/Content/UploadedFolder"), excelm.FileName);
                //if (System.IO.File.Exists(filePath))
                //    System.IO.File.Delete(filePath);
                //Request.Files["xlsFile"].SaveAs(filePath);
                //Application app = new Application();

                //tblExcelMaster excelm = new tblExcelMaster();
                //excelm.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                //if (excelm.FileName != null)
                //    excelm.FileName = Base64Encode(file.ToString());
                //excelm.CreatedDate = DateTime.Now;
                //excelm.Status = "Active";
                //scheModel.tblExcelMasters.Add(excelm);
                //scheModel.SaveChanges();

                //Byte[] bytes = System.IO.File.ReadAllBytes("path");
                //String file = Convert.ToBase64String(bytes)
                //string path = Server.MapPath("~/Content/UploadedFolder");
                //string filename = excelm.MasterExcel_Id + ".xls";
                //set the image path
                //string filepath = Path.Combine(path, filename);
                //byte[] fileinbytes = Convert.FromBase64String(excelm.FileName);
                //System.IO.File.WriteAllBytes(filepath, fileinbytes);

                //Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
                //Microsoft.Office.Interop.Excel.Workbook wb = app.WorkBooks;
                //return Json("Save Success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) { throw; }

        }

        public ActionResult MasterExcelOpen(string File)
        {
            try
            {
                string path = Server.MapPath("~/Content/UploadedFolder");
                string childpath = "";
                string[] filePaths = Directory.GetFiles(path, "*.xls");
                foreach (var item in filePaths)
                {
                    if (File == Path.GetFileName(item))
                    {
                        childpath = Path.Combine(Server.MapPath("~/Content/UploadedFolder/File"), Path.GetFileName(item));
                        System.IO.File.Copy(item, childpath, true);
                    }
                }

                //string patha = Server.MapPath("~/Content/Loan Calculation.xlsx");
                Application excelApp = new Application();
                excelApp.Visible = true;
                //excelApp.FileValidation.
               
                Workbooks books = excelApp.Workbooks;
                Workbook sheet = books.Open(childpath);
                excelApp.ExecuteExcel4Macro("SHOW.TOOLBAR(\"Ribbon\",False)");

                //Application excelApp = new Application();


                //excelApp.ExecuteExcel4Macro("Show.ToolBar("+"Ribbon"+",False)");
                //excelApp.CommandBars.ExecuteMso("MinimizeRibbon");
                //excelApp.CommandBars.ExecuteMso("HideRibbon");
                // System.IO.File.Open(childpath, FileMode.Open);
                //Application excel = new Application();

                //var lstEmp = scheModel.tblExcelMasters.Where(x => x.MasterExcel_Id == id).FirstOrDefault();
                ViewBag.ChildFileList = Childfilelist();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult ChildExcelOpen(string File)
        {
            try
            {
                string path = Server.MapPath("~/Content/UploadedFolder/File");
                string openPath = "";
                string[] filePaths = Directory.GetFiles(path, "*.xls");
                foreach (var item in filePaths)
                {
                    if (File == Path.GetFileName(item))
                    {
                        openPath = Path.Combine(Server.MapPath("~/Content/UploadedFolder/File"), Path.GetFileName(item));
                    }
                }

                //string patha = Server.MapPath("~/Content/Loan Calculation.xlsx");
                var excelApp = new Application();
                excelApp.Visible = true;

                Workbooks books = excelApp.Workbooks;
                Workbook sheet = books.Open(openPath);

                // System.IO.File.Open(childpath, FileMode.Open);
                //Application excel = new Application();

                //var lstEmp = scheModel.tblExcelMasters.Where(x => x.MasterExcel_Id == id).FirstOrDefault();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}