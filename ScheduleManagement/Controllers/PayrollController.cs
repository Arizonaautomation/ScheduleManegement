using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.CommonClass;
using TrainningManagement.Models;
using TrainningManagement.SessionValidate;

namespace TrainningManagement.Controllers
{
    #region
    [ActionAuthorize]
    #endregion
    public class PayrollController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();
        AuditTrail at = new AuditTrail();
        // GET: Payroll
        public ActionResult Index()
        {
            ViewBag.Groupchild = grpchildList();
            ViewBag.LeaveList = BindData();
            return View();
        }
        public List<tblAccessGroupChild> grpchildList()
        {
            var EmployeeId = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;

            var AccessGroupChildDetail = scheModel.tblEmployees
                        .Join(
                              scheModel.tblAccessGroupChilds,
                              E => E.group_Id,
                              GC => GC.group_Id,
                              (E, GC) => new { E, GC })
                        .Join(
                              scheModel.tblAccessMenuMasters,
                              GCC => GCC.GC.menu_Id,
                              MM => MM.menu_Id,
                              (GCC, MM) => new
                              {
                                  EmployeeID = GCC.E.Employee_Id,
                                  MenuId = MM.menu_Id,
                                  MenuName = MM.menu_Name,
                                  AccessCreate = GCC.GC.access_Create,
                                  AccessModify = GCC.GC.access_Modify,
                                  AccessDelete = GCC.GC.access_Delete,
                                  AccessView = GCC.GC.access_View,
                              }).Where(x => (x.AccessCreate == "true" || x.AccessModify == "true" || x.AccessDelete == "true" || x.AccessView == "true") && x.EmployeeID == EmployeeId).ToList();
            List<tblAccessGroupChild> grpChildAccessList = new List<tblAccessGroupChild>();
            foreach (var item in AccessGroupChildDetail)
            {
                tblAccessGroupChild grpchild = new tblAccessGroupChild();
                grpchild.menu_Id = item.MenuId;
                grpchild.access_Create = item.AccessCreate;
                grpchild.access_Modify = item.AccessModify;
                grpchild.access_Delete = item.AccessDelete;
                grpchild.access_View = item.AccessView;
                grpChildAccessList.Add(grpchild);
            }

            return grpChildAccessList;
        }

        //public ActionResult HeadList()
        //{
        //    var departmentId = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;

        //    var HeadList = scheModel.tblEmployees.Where(x => x.HeadType != null && x.Department_UserType == "Head" && x.Employee_Department == departmentId).ToList();
        //    return Json(new { data = HeadList }, JsonRequestBehavior.AllowGet);
        //}

        public List<tblPayroll> BindData()
        {
            List<tblPayroll> LeaveList = new List<tblPayroll>();
            var EmployeeId = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
            LeaveList = scheModel.tblPayrolls.Where(x => x.CreatedBy == EmployeeId || x.Head_Id == EmployeeId).ToList();
            return LeaveList;
        }

        public ActionResult PandingLeave()
        {
            var EmployeeId = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
            var result = scheModel.tblEmployees
                .Join(
                              scheModel.tblPayrolls,
                              A => A.Employee_Id,
                              B => B.UserId,
                              (A, B) => new
                              {
                                  EmployeeID = A.Employee_Id,
                                  ApproveLeaves = B.NoOfDays,
                                  //TotalLeaves = A.TotalNoOfLeaves,
                                  status = B.Status,
                              }).Where(x => x.status == "Approve" && x.EmployeeID == EmployeeId).ToList();

            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }
        public ActionResult LeaveCreation(tblPayroll LeaveCreation)
        {
            try
            {
                LeaveCreation.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                //var isExistm = scheModel.tblPayrolls.Any(x => x.Payroll_Id == LeaveCreation.Payroll_Id);
                // if (!isExistm)
                //{
                LeaveCreation.CreatedDatetime = DateTime.Now;
                scheModel.tblPayrolls.Add(LeaveCreation);
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], ((tblEmployee)(Session["EmployeeData"])).Employee_Id, "Payroll", "NA", "New Leave Created", LeaveCreation.otherdata.Remark);
                return Json("Save Success", JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    return Json("Updated Success", JsonRequestBehavior.AllowGet);
                //}
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult LeaveAppEdit(long id)
        {
            try
            {
                var lstLeaveApp = scheModel.tblPayrolls.Where(x => x.Payroll_Id == id).FirstOrDefault();
                return Json(lstLeaveApp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

    }
}