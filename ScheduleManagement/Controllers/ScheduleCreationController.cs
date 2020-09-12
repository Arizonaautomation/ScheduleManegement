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
                                 }).Where(y => y.MachineDepId == sessionData.Employee_Department && y.WfMoveStep == 0 && y.inst_Equip_ID == 1).ToList();
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
    }
}