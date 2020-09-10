using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.CommonClass;
using TrainningManagement.Models;
using TrainningManagement.Pages;
using TrainningManagement.SessionValidate;
namespace TrainningManagement.Controllers
{
    #region
    [ActionAuthorize]
    #endregion
    public class AssetController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();
        GenericModel GM = new GenericModel();
        LoginSessionl sessionObj = new LoginSessionl();
        AuditTrail at = new AuditTrail();
        common cm = new common();
        // GET: Asset
        public ActionResult Index()
        {
            try
            {
                //var mastermodel = Generic();
                ////sessionObj = ((TrainningManagement.Models.tblEmployee)(Session["EmployeeData"])).Employee_Type;
                //if (((TrainningManagement.Models.tblEmployee)(Session["EmployeeData"])).Employee_Type == "Admin")
                //    ViewBag.Employeename = employeeData(mastermodel.Employee.Where(x => x.Employee_Type == "User").ToList());
                //if (((TrainningManagement.Models.tblEmployee)(Session["EmployeeData"])).Employee_Type == "Reviewer")
                //    ViewBag.Employeename = employeeData(mastermodel.Employee.Where(x => x.Employee_Type == "Aprover").ToList());

                //ViewBag.Machine = machineData(mastermodel.Asset);
            }
            catch (Exception)
            {

                throw;
            }
            ViewBag.Groupchild = grpchildList();
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
                              }).Where(x => (x.AccessCreate == "True" || x.AccessModify == "True" || x.AccessDelete == "True" || x.AccessView == "True") && x.EmployeeID == EmployeeId).ToList();
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

        private List<tblMachineCreation> machineData(List<tblMachineCreation> asset)
        {
            List<tblMachineCreation> tm = new List<tblMachineCreation>();
            foreach (var item in asset)
            {
                tm.Add(new tblMachineCreation
                {
                    Machine_Id = item.Machine_Id,
                    Machine_Name = item.Machine_Name
                });
            }
            return tm;
        }
        private ScheduleMachine oldentrydata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.ScheduleMachines.Single(p => p.Id == id);
        }
        public ActionResult CreateSchedule(ScheduleMachine sm)
        {
            // var olddata = scheModel.ScheduleMachines.Where(x => x.Id == sm.Id);
            var isExist = scheModel.ScheduleMachines.Any(x => x.Id == sm.Id);
            if (!isExist)
            {
                sm.CreatedDate = DateTime.Now;
                sm.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                sm.CreatedDepartment = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                sm.StartScheduleDate = Convert.ToDateTime(sm.StartScheduleDate);
                sm.EndScheduleDate = Convert.ToDateTime(sm.EndScheduleDate);
                scheModel.ScheduleMachines.Add(sm);
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.Id, "ScheduleMachine", "NA", "Created Asset", sm.otherdata.Remark);
                return Json("Schedule Generate", JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (sm.Status == "Pass")
                {
                    sm.ApprovedDate = DateTime.Now;
                }
                ScheduleMachine olditem = oldentrydata(sm.Id);

                sm.CreatedDate = DateTime.Now;
                sm.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                //sm.EmployeeHead = ((tblEmployee)(Session["EmployeeData"])).UserHead_Id;
                sm.StartScheduleDate = Convert.ToDateTime(sm.StartScheduleDate);
                sm.EndScheduleDate = Convert.ToDateTime(sm.EndScheduleDate);
                scheModel.ScheduleMachines.Attach(sm);
                //scheModel.Entry(sm).State = EntityState.Modified;
                //foreach (var olditem in olddata)
                // {
                if (olditem.MachineId != sm.MachineId)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.MachineId.ToString(), sm.MachineId.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.MachineId).IsModified = true;
                }
                if (olditem.MachineLocation != sm.MachineLocation)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.MachineLocation.ToString(), sm.MachineLocation.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.MachineLocation).IsModified = true;
                }
                if (olditem.ScheduleType != sm.ScheduleType)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.ScheduleType.ToString(), sm.ScheduleType.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.ScheduleType).IsModified = true;
                }
                if (olditem.StartScheduleDate != sm.StartScheduleDate)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.StartScheduleDate.ToString(), sm.StartScheduleDate.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.StartScheduleDate).IsModified = true;
                }
                if (olditem.EndScheduleDate != sm.EndScheduleDate)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.EndScheduleDate.ToString(), sm.EndScheduleDate.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.EndScheduleDate).IsModified = true;
                }
                if (olditem.AsignTo != sm.AsignTo)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.AsignTo.ToString(), sm.AsignTo.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.AsignTo).IsModified = true;
                }
                if (olditem.Status != sm.Status)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.Status.ToString(), sm.Status.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.Status).IsModified = true;
                }
                if (olditem.AsignType != sm.AsignType)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.AsignType.ToString(), sm.AsignType.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.AsignType).IsModified = true;
                }
                if (olditem.ApprovedDate != sm.ApprovedDate)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.ApprovedDate.ToString(), sm.ApprovedDate.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.ApprovedDate).IsModified = true;
                }
                if (olditem.CreatedBy != sm.CreatedBy)
                {
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.CreatedBy.ToString(), sm.CreatedBy.ToString(), sm.otherdata.Remark);
                    scheModel.Entry(sm).Property(x => x.CreatedBy).IsModified = true;
                }

                at.InsrtHistory((tblEmployee)Session["EmployeeData"], sm.MachineId, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), sm.otherdata.Remark);
                //}
                scheModel.SaveChanges();
                return Json("Update Success", JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult bindData()
        {
            List<ScheduleMachine> lstSM = new List<ScheduleMachine>();
            List<tblAccessGroupChild> grpchild = new List<tblAccessGroupChild>();
            var lstBndData = getDataByUser();

            foreach (var item in lstBndData)
            {
                grpchild = grpchildList();
                foreach (var child in grpchild)
                {
                    if (child.menu_Id == 12)
                    {
                        if (child.access_View == "true")
                        {
                            //    "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                            //+ " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=deleteSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                            //+ " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";
                        }
                        if (child.access_Modify == "true" && child.access_Delete == "false")
                        {
                            item.otherdata.Button =
                               "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                           + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";

                        }
                        if (child.access_Delete == "true" && child.access_Modify == "false")
                        {
                            item.otherdata.Button = " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" data-toggle=\"modal\" data-target=\"#AddRemark\" onclick=GetID(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                           + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";

                        }
                        if (child.access_Modify == "true" && child.access_Delete == "true")
                        {
                            item.otherdata.Button = "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                            + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" data-toggle=\"modal\" data-target=\"#AddRemark\" onclick=GetID(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                            + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";
                        }
                    }
                    else if (child.menu_Id == 13)
                    {
                        if (child.access_View == "true")
                        {
                            item.otherdata.Button = "";
                            //    "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                            //+ " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=deleteSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                            //+ " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";
                        }
                        if (child.access_Modify == "true" && child.access_Delete == "false")
                        {
                            item.otherdata.Button =
                               "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                           + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";

                        }
                        if (child.access_Delete == "true" && child.access_Modify == "false")
                        {
                            item.otherdata.Button = " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" data-toggle=\"modal\" data-target=\"#AddRemark\" onclick=GetID(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                           + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";

                        }
                        if (child.access_Modify == "true" && child.access_Delete == "true")
                        {
                            item.otherdata.Button = "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                            + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" data-toggle=\"modal\" data-target=\"#AddRemark\" onclick=GetID(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                            + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";
                        }
                    }
                    else if (child.menu_Id == 14)
                    {
                        if (child.access_View == "true")
                        {
                            item.otherdata.Button = "";
                            //    "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                            //+ " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=deleteSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                            //+ " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";
                        }
                        if (child.access_Modify == "true" && child.access_Delete == "false")
                        {
                            item.otherdata.Button =
                               "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                           + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";

                        }
                        if (child.access_Delete == "true" && child.access_Modify == "false")
                        {
                            item.otherdata.Button = " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" data-toggle=\"modal\" data-target=\"#AddRemark\" onclick=GetID(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                           + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";

                        }
                        if (child.access_Modify == "true" && child.access_Delete == "true")
                        {
                            item.otherdata.Button = "<button class=\"btn btn-minier btn-white btn-info btn-bold\" onclick=editSchedule(\"" + Convert.ToString(item.Id) + "\") title=\"Edit Service\" ><i class=\"ace-icon fa fa-pencil bigger-80\"></i></button>"
                            + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" data-toggle=\"modal\" data-target=\"#AddRemark\" onclick=GetID(\"" + Convert.ToString(item.Id) + "\") title=\"Delete Service\" ><i class=\"ace-icon fa fa-trash bigger-80\"></i></button>"
                            + " <button class=\"btn btn-minier btn-white btn-danger btn-bold\" onclick=ScheduleMail(\"" + Convert.ToString(item.Id) + "\") title=\"Send Email\" ><i class=\"ace-icon fa fa-envelope bigger-80\"></i></button>";
                        }
                    }
                }
                lstSM.Add(item);
            }
            return Json(new { aaData = lstSM }, JsonRequestBehavior.AllowGet);
        }
        public List<ScheduleMachine> getDataByUser()
        {
            try
            {
                var UserId = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                //var UserType = ((tblEmployee)(Session["EmployeeData"])).Employee_Type;
                long deptId = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                List<ScheduleMachine> lstSchedule = new List<ScheduleMachine>();
                //if(UserType == "Admin")
                //    lstSchedule = scheModel.ScheduleMachines.ToList();
                //if (UserType == "User")
                //    lstSchedule = scheModel.ScheduleMachines.Where(x => x.CreatedDepartment == deptId ||x.EmployeeHead== UserId).ToList();
                //if (UserType == "Reviewer")
                //    lstSchedule = scheModel.ScheduleMachines.Where(x => x.AsignType == "Reviewer" && x.CreatedDepartment == deptId || x.EmployeeHead == UserId).ToList();//&& x.AsignTo == UserId
                //if (UserType == "Approver")
                //    lstSchedule = scheModel.ScheduleMachines.Where(x => x.AsignType == "Approver" && x.CreatedDepartment == deptId || x.EmployeeHead == UserId).ToList(); //&& x.AsignTo == UserId
                return lstSchedule;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<tblEmployee> employeeData(List<tblEmployee> employee)
        {
            List<tblEmployee> te = new List<tblEmployee>();
            foreach (var item in employee)
            {
                te.Add(new tblEmployee
                {
                    Employee_Name = item.Employee_Name,
                    Employee_Id = item.Employee_Id,
                });
            }
            return te;
        }
        public ActionResult editSchedule(int id)
        {
            try
            {
                var scheduleData = scheModel.ScheduleMachines.Where(x => x.Id == id).FirstOrDefault();
                //at.InsrtHistory((tblEmployee)Session["EmployeeData"], scheduleData.Id, "ScheduleMachine", "CreatedAsset", "Edit Asset", "Edit Asset Success");
                return Json(scheduleData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                // at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "ScheduleMachine", "Created Asset", "edit Asset", ex.Message);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult deleteSchedule(int id, string remark)
        {
            try
            {
                var deleteLine = scheModel.ScheduleMachines.Where(x => x.Id == id).FirstOrDefault();
                scheModel.Entry(deleteLine).State = EntityState.Deleted;
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], deleteLine.Id, "ScheduleMachine", "Created Asset", "Delete Asset", remark);
                return Json("Deleted Successfuly", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "ScheduleMachine", "Created Asset", "Delete Asset", ex.Message);
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }
        public GenericModel Generic()
        {

            GM.Asset = scheModel.tblMachineCreations.ToList();
            GM.Department = scheModel.tblDepartments.ToList();
            GM.Designation = scheModel.tblDesignations.ToList();
            GM.Employee = scheModel.tblEmployees.ToList();
            GM.Schedule = scheModel.tblScheduleMasters.ToList();
            return GM;
        }
        //public ActionResult getAsignType(int AsignId)
        //{
        //    try
        //    {
        //        var asnType = scheModel.tblEmployees.Where(x => x.Employee_Id == AsignId).FirstOrDefault().Employee_Type;
        //        return Json(asnType, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {

        //        return Json(ex.Message, JsonRequestBehavior.AllowGet);
        //    }
        //}
        public ActionResult getAsignEmail(int id)
        {
            try
            {
                var asnEmail = scheModel.ScheduleMachines
                     .Join(
                     scheModel.tblEmployees,
                     S => S.AsignTo,
                     E => E.Employee_Id.ToString(), (S, E) => new
                     {
                         Id = S.Id,
                         EmailId = E.Employee_Email,
                     }).Where(x => x.Id == id).FirstOrDefault();

                return Json(asnEmail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }


        }

        public string sendMail(common cm)
        {
            try
            {
                var result = cm.sendMail(cm);
                return result;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        public ActionResult partialFormForAllUser()
        {
            try
            {
                var mastermodel = Generic();
                //var userType = ((tblEmployee)(Session["EmployeeData"])).Employee_Type;
                long DeptId = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                //sessionObj = ((TrainningManagement.Models.tblEmployee)(Session["EmployeeData"])).Employee_Type;
                //if (userType == "User")
                //{
                //    ViewBag.Employeename = employeeData(mastermodel.Employee.Where(x => x.Employee_Type == "Reviewer" && x.Employee_Department == DeptId).ToList());
                //    var data = mastermodel.Asset.Where(x => x.Machine_Department == DeptId).ToList();
                //    ViewBag.Machine = machineData(data);
                //}
                //if (userType == "Reviewer")
                //{
                //    ViewBag.Employeename = employeeData(mastermodel.Employee.Where(x => x.Employee_Type == "Approver" && x.Employee_Department == DeptId).ToList());
                //    var data = mastermodel.Asset.Where(x => x.Machine_Department == DeptId).ToList();
                //    ViewBag.Machine = machineData(data);

                //}
                //if (userType == "Approver")
                //{
                //    ViewBag.Employeename = employeeData(mastermodel.Employee.Where(x => x.Employee_Type == "Reviewer" && x.Employee_Department == DeptId).ToList());
                //    var data = mastermodel.Asset.Where(x => x.Machine_Department == DeptId).ToList();
                //    ViewBag.Machine = machineData(data);
                //}
                //if (userType == "Admin")
                //{
                //    ViewBag.Employeename = employeeData(mastermodel.Employee.ToList());
                //    ViewBag.Machine = machineData(mastermodel.Asset);
                //}
                return PartialView();
            }
            catch (Exception)
            {

                throw;
            }

        }
        public ActionResult Reviewer()
        {
            ViewBag.Groupchild = grpchildList();
            return View();
        }
        public ActionResult Approver()
        {
            ViewBag.Groupchild = grpchildList();
            return View();
        }
    }

}
public class Data
{
    public List<string> Olddata { get; set; }
}