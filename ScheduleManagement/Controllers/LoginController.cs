using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.Models;
using TrainningManagement.CommonClass;

namespace TrainningManagement.Controllers
{

    public class LoginController : Controller
    {
        // GET: Login
        long Id = 1;
        String Name = "SuperAdmin";
        String Password = "SupAdmin@123";
        AuditTrail at = new AuditTrail();
        dbScheduleModel dbSche = new dbScheduleModel();
        public ActionResult Index()
        {
            //MasterController mc = new MasterController();
            //ViewBag.Departmet = mc.fillmachineDropDown();
            return View();
        }
        public ActionResult LoginData(tblEmployee tm)
        {
            var empldata = dbSche.tblEmployees.Where(x => x.Employee_UserName == tm.Employee_UserName && x.Employee_Password == tm.Employee_Password && x.Status=="Active").FirstOrDefault();
            if (empldata != null)
            {
                //if (empldata.FirstTimeLoginStatus == "FALSE")
                //{
                //    //return RedirectToAction("Index", "Login");
                //    return Json(empldata, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //    Session["EmployeeData"] = empldata;
                //    var x = Session["EmployeeData"];
                //    return RedirectToAction("Index", "Home", new { returnUrl = (this.HttpContext.Request).Path });
                //}
                Session["EmployeeData"] = empldata;
                var x = Session["EmployeeData"];
                //at.InsrtHistory(empldata, "Logout", "Login", "login Success");
                return Json(empldata, JsonRequestBehavior.AllowGet);

            }
            else
            {
                //InsrtHistory(empldata, "Login", "login faill");
                return RedirectToAction("Index", "Login");
            }
        }

        public ActionResult ChangePasswordEmployee(tblEmployee Emp)
        {
            var data = dbSche.tblEmployees.FirstOrDefault(x => x.Employee_Id == Emp.Employee_Id);
            try
            {
                if (data != null)
                {
                    data.Employee_Password = Emp.Employee_Password;
                    data.FirstTimeLoginStatus = Emp.FirstTimeLoginStatus;
                    dbSche.SaveChanges();
                  //  at.InsrtHistory(data, "Old Password", "Generate New Password", "Change Password Success");
                }
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
               // at.InsrtHistory(data, "Old Password", "Generate Change Password", e.Message);
                throw;
            }
        }
        public ActionResult LogOut()
        {
            try
            {
               // at.InsrtHistory((tblEmployee)Session["EmployeeData"], "Login", "LogOut", "log Out");
                Session["EmployeeData"] = null;
                return RedirectToAction("Index", "Login");
            }
            catch (Exception ex)
            {
              //  at.InsrtHistory((tblEmployee)Session["EmployeeData"],"Login", "LogOut", ex.Message);
                throw;
            }
        }

    }
}