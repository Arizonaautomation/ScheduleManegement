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
    public class ScheduleCreationController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();
        AuditTrail at = new AuditTrail();
        // GET: ScheduleCreation
        public ActionResult Index()
        {
            ViewBag.Groupchild = grpchildList();
            ViewBag.Schedule = ShceduleList();
            ViewBag.InstrumentList = lstMachine();
            return View();
        }
        public List<tblAccessGroupChild> grpchildList()
        {
            List<tblAccessGroupChild> grpChildAccessList = new List<tblAccessGroupChild>();
            var SessionData = ((tblEmployee)(Session["EmployeeData"]));
            if (SessionData.SiteId == null)
            {
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
                                 }).Where(x => (x.AccessCreate == "True" || x.AccessModify == "True" || x.AccessDelete == "True" || x.AccessView == "True") && x.EmployeeID == SessionData.Employee_Id).ToList();
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

            }
            else
            {
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
                                     siteId = GCC.E.SiteId
                                 }).Where(x => (x.AccessCreate == "True" || x.AccessModify == "True" || x.AccessDelete == "True" || x.AccessView == "True") && x.EmployeeID == SessionData.Employee_Id && x.siteId == SessionData.SiteId).ToList();
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

            }

            return grpChildAccessList;
        }

        public List<Machine> lstMachine()
        {
            try
            {
                List<Machine> lstMachine = new List<Machine>();
                var sessionData = ((tblEmployee)(Session["EmployeeData"]));
                if (sessionData.SiteId == null)
                {
                    var machine = scheModel.tblMachineCreations
                           .Join(
                                 scheModel.tblDepartments,
                                 M => M.Machine_Department,
                                 D => D.Department_Id,
                                 (M, D) => new { M, D })
                           .Join(
                                 scheModel.tblWorkFlowMasters,
                                 MWF => MWF.M.Machine_Workflow,
                                 WF => WF.WorkFlowId,
                                 (MWF, WF) => new
                                 {
                                     machine_Id = MWF.M.Machine_Id,
                                     machineId = MWF.M.MachineID,
                                     machineName = MWF.M.Machine_Name,
                                     MachineDepId = MWF.M.Machine_Department,
                                     machineDepartment = MWF.D.Department_Name,
                                     machineLocation = MWF.M.Machine_Location,
                                     machineWorkflow = WF.WorkFlowName,
                                     machineStatus = MWF.M.Status,
                                     StatusId = MWF.M.Instru_Equip_StatusId,
                                     WfMoveStep = MWF.M.WfMovedStep,
                                     inst_Equip_ID = MWF.M.Instru_Equip_StatusId,
                                 }).Where(y => y.MachineDepId == sessionData.Employee_Department && y.machineStatus == "Active" && y.inst_Equip_ID == 1).ToList();
                    Machine Newmachine;
                    foreach (var item in machine)
                    {
                        Newmachine = new Machine();
                        Newmachine.Machine_Id = item.machine_Id;
                        Newmachine.MachineID = item.machineId;
                        Newmachine.Machine_Name = item.machineName;
                        Newmachine.DepartmentName = item.machineDepartment;
                        Newmachine.Machine_Location = item.machineLocation;
                        Newmachine.WorkflowName = item.machineWorkflow;
                        Newmachine.Status = item.machineStatus;
                        Newmachine.Instru_Equip_StatusId = Convert.ToInt64(item.StatusId);
                        lstMachine.Add(Newmachine);
                    }
                }
                else
                {
                    //var machine = scheModel.tblMachineCreations.Where(y => y.Machine_Department == sessionData.Employee_Department && y.SiteId == sessionData.SiteId).ToList();
                    //for (int i = 0; i < machine.Count; i++)
                    //{
                    //    lstMachine.Add(machine[i]);
                    //}
                }
                return lstMachine;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        public List<ScheduleMachine> ShceduleList()
        {
            List<ScheduleMachine> SList = new List<ScheduleMachine>();
            return SList;
        }

        private ScheduleMachine oldScheduledata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.ScheduleMachines.Single(p => p.Id == id);
        }


        public ActionResult ScheduleCreation(ScheduleMachine SM)
        {
            try
            {
                DateTime date;
                var Count = 0;
                int diff = Convert.ToDateTime(SM.TillDate).Subtract(Convert.ToDateTime(SM.InstallationDate)).Days;
                if (SM.FrequencyType == "15 Days")
                    Count = diff / 15;
                if (SM.FrequencyType == "1 month")
                    Count = diff / 30;
                if (SM.FrequencyType == "3 months")
                    Count = diff / 90;
                if (SM.FrequencyType == "6 months")
                    Count = diff / 180;

                date = Convert.ToDateTime(SM.InstallationDate);
                SM.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                SM.CreatedDepartment = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;

                var wfstep = scheModel.tblMachineCreations.
                           Join(
                                scheModel.tblWorkFlowChilds,
                                M => M.Machine_Workflow,
                                WC => WC.WorFlowId,
                                (M, WC) => new
                                {
                                    MachineId = M.Machine_Id,
                                    wfID = WC.WorFlowId,
                                    wfChildId = WC.WFChild_Id,
                                    flowStep = WC.FlowStep,
                                }
                    ).Where(x => x.MachineId == SM.MachineId).ToList();
                long WorkflowID = 0;
                foreach (var item in wfstep)
                {
                    WorkflowID = Convert.ToInt64(item.wfID);
                    if (item.flowStep == "Creater")
                    {
                        SM.ScheduleMovedStep = item.wfChildId + 1;
                        break;
                    }
                    else
                    {
                        SM.ScheduleMovedStep = item.wfChildId;
                        break;
                    }
                }
                var wfstepUpdated = scheModel.tblWorkFlowChilds.Where(x => x.WFChild_Id == SM.ScheduleMovedStep && x.WorFlowId == WorkflowID).FirstOrDefault();
                if (wfstepUpdated == null)
                    SM.Status = "Completed";
                else
                    SM.Status = wfstepUpdated.FlowStep;
                var St = 1;
                for (int i = 0; i < Count; i++)
                {
                    if (SM.FrequencyType == "15 Days")
                        date = date.AddDays(15);
                    if (SM.FrequencyType == "1 month")
                        date = date.AddMonths(1);
                    if (SM.FrequencyType == "3 months")
                        date = date.AddMonths(3);
                    if (SM.FrequencyType == "6 months")
                        date = date.AddMonths(6);
                    if (St == 0)
                        SM.Status = "NULL";
                    SM.ScheduleDueDate = date;
                    SM.BeforegracedDate = date.Subtract(TimeSpan.FromDays(Convert.ToInt64(SM.BGracedPeriod)));
                    SM.AftergracedDate = date.AddDays(Convert.ToInt64(SM.AGrancedPeriod));
                    SM.CreatedDate = DateTime.Now;
                    scheModel.ScheduleMachines.Add(SM);
                    scheModel.SaveChanges();
                    St = 0;
                }
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], SM.Id, "ScheduleMachine", "NA", "New Schedule Created", SM.otherdata.Remark);
                return Json("Save Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }
        }

        public ActionResult ScheduleEdit(long id)
        {
            try
            {
                var lstEmp = scheModel.ScheduleMachines.Where(x => x.Id == id).FirstOrDefault();
                return Json(lstEmp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult ScheduleDeactive(long Id, string remark)
        {
            try
            {
                ScheduleMachine olditem = oldScheduledata(Id);
                ScheduleMachine Schedule = new ScheduleMachine();
                Schedule.Id = Id;
                Schedule.Status = "Deactive";
                scheModel.ScheduleMachines.Attach(Schedule);
                scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.Status.ToString(), Schedule.Status.ToString(), remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), remark);
                scheModel.SaveChanges();
                return Json("Data Deactive Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], Id, "Employee", "NA", "Deactive", ex.Message);
                throw;
            }
        }

    }
}