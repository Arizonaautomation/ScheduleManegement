using System;
using System.Collections.Generic;
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
            ViewBag.Mlist = masterList();
            return View();
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public List<tblExcelMaster> masterList()
        {
            List<tblExcelMaster> List = new List<tblExcelMaster>();
            var Data = scheModel.tblExcelMasters.ToList();
            tblExcelMaster data = new tblExcelMaster();
            foreach (var item in Data)
            {
                data.MasterExcel_Id = item.MasterExcel_Id;
                data.FileName = Base64Decode(item.FileName);
                List.Add(data);
            }
            return List;
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public ActionResult ExcelCreation(tblExcelMaster excelm)
        {
            try
            {
                excelm.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                if (excelm.FileName != null)
                    excelm.FileName = Base64Encode(excelm.FileName);
                excelm.CreatedDate = DateTime.Now;
                excelm.Status = "Active";
                scheModel.tblExcelMasters.Add(excelm);
                scheModel.SaveChanges();
                return Json("Save Success", JsonRequestBehavior.AllowGet);

            }
            catch (Exception e) { throw; }

        }

        public ActionResult ExcelEdit(long id)
        {
            try
            {
                var lstEmp = scheModel.tblExcelMasters.Where(x => x.MasterExcel_Id == id).FirstOrDefault();
                return Json(lstEmp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}