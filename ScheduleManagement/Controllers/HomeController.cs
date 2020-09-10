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

    public class HomeController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();
        public ActionResult Index()
        {
            Session["MenuList"] = MenuList();
            return View();
        }

        public List<tblAccessMenuMaster> MenuList()
        {
            var EmployeeId = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;

            var AccessGroupChildDetail = scheModel.tblAssignEmployeeGroups
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
                                  EmployeeID = GCC.E.employee_Id,
                                  MenuId = MM.menu_Id,
                                  MenuName = MM.menu_Name,
                                  MenuLink = MM.menu_Link,
                                  MenuIcon = MM.menu_Icon,
                                  AccessCreate=GCC.GC.access_Create,
                                  AccessModify=GCC.GC.access_Modify,
                                  AccessDelete=GCC.GC.access_Delete,
                                  AccessView=GCC.GC.access_View,
                              }).Where(x => (x.AccessCreate == "True" || x.AccessModify == "True" || x.AccessDelete == "True" || x.AccessView == "True") && x.EmployeeID == EmployeeId).Distinct().ToList();
            List<tblAccessMenuMaster> MenuList = new List<tblAccessMenuMaster>();
            foreach (var item in AccessGroupChildDetail)
            {
                tblAccessMenuMaster menu = new tblAccessMenuMaster();
                menu.menu_Id = item.MenuId;
                menu.menu_Name = item.MenuName;
                menu.menu_Link = item.MenuLink;
                menu.menu_Icon = item.MenuIcon;
                MenuList.Add(menu);
            }

            return MenuList;
        }
        public ActionResult GetNotifiedRecord()
        {
            try
            {
                var endDate = DateTime.Today.AddDays(15);
                var data = scheModel.ScheduleMachines.Where(x => x.ApprovedDate == endDate).ToList();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult AddTrainning()
        {
            return Json("Success", JsonRequestBehavior.AllowGet);
        }
    }
}