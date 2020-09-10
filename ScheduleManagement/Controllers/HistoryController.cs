using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.Models;
using TrainningManagement.SessionValidate;

namespace TrainningManagement.Controllers
{
    #region
    [ActionAuthorize]
    #endregion

    public class HistoryController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();

        // GET: History
        public ActionResult Index()
        {
            ViewBag.Department = departmentList();
            // ViewBag.log = BindData("", "", 0);
            return View();
        }

        public ActionResult BindData()
        {
            var loglist = scheModel.tblHistories.ToList();
            return Json(new { data = loglist }, JsonRequestBehavior.AllowGet);

        }


        public List<tblDepartment> departmentList()
        {
            var lstDprt = scheModel.tblDepartments.ToList();
            List<tblDepartment> lsdept = new List<tblDepartment>();
            foreach (var item in lstDprt)
            {
                lsdept.Add(new tblDepartment
                {
                    Department_Id = item.Department_Id,
                    Department_Name = item.Department_Name
                });

            }
            return lsdept;
        }


        public ActionResult BindSearchData(string fromdate, string toDate, long DeparmentId)
        {
            DateTime Fromdate = Convert.ToDateTime(fromdate);
            DateTime ToDate = Convert.ToDateTime(toDate);

            var loglist = scheModel.tblHistories.Where(x => (x.EntryDatetime >= Fromdate && x.EntryDatetime <= ToDate) && x.Department_Id == DeparmentId).ToList();
            return Json(new { data = loglist }, JsonRequestBehavior.AllowGet);
        }



    }
}