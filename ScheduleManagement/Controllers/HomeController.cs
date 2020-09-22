using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public class HomeController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();
        AuditTrail at = new AuditTrail();
        public ActionResult Index()
        {
            Session["MenuList"] = MenuList();
            ViewBag.Groupchild = grpchildList();
            ViewBag.InstrumentReviewer = ReviewerInstrumentList();
            ViewBag.InstrumentApprover = ApproverInstrumentList();
            ViewBag.EquipmentReviewer = ReviewerEquipmentList();
            ViewBag.EquipmentApprover = ApproverEquipmentList();
            ViewBag.ScheduleReviewer = ReviewerScheduleList();
            ViewBag.ScheduleApprover = ApproverScheduleList();
            ViewBag.ScheduleExecution = ExecutionScheduleList();

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
                                     AccessReview = GCC.GC.access_Review,
                                     AccessApprove = GCC.GC.access_Approve,
                                 }).Where(x => (x.AccessCreate == "True" || x.AccessModify == "True" || x.AccessDelete == "True" || x.AccessView == "True" || x.AccessReview == "True" || x.AccessApprove == "True") && x.EmployeeID == SessionData.Employee_Id).ToList();
                foreach (var item in AccessGroupChildDetail)
                {
                    tblAccessGroupChild grpchild = new tblAccessGroupChild();
                    grpchild.menu_Id = item.MenuId;
                    grpchild.access_Create = item.AccessCreate;
                    grpchild.access_Modify = item.AccessModify;
                    grpchild.access_Delete = item.AccessDelete;
                    grpchild.access_View = item.AccessView;
                    grpchild.access_Review = item.AccessReview;
                    grpchild.access_Approve = item.AccessApprove;
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
                                     AccessReview = GCC.GC.access_Review,
                                     AccessApprove = GCC.GC.access_Approve,
                                     siteId = GCC.E.SiteId
                                 }).Where(x => (x.AccessCreate == "True" || x.AccessModify == "True" || x.AccessDelete == "True" || x.AccessView == "True" || x.AccessReview == "True" || x.AccessApprove == "True") && x.EmployeeID == SessionData.Employee_Id && x.siteId == SessionData.SiteId).ToList();
                foreach (var item in AccessGroupChildDetail)
                {
                    tblAccessGroupChild grpchild = new tblAccessGroupChild();
                    grpchild.menu_Id = item.MenuId;
                    grpchild.access_Create = item.AccessCreate;
                    grpchild.access_Modify = item.AccessModify;
                    grpchild.access_Delete = item.AccessDelete;
                    grpchild.access_View = item.AccessView;
                    grpchild.access_Review = item.AccessReview;
                    grpchild.access_Approve = item.AccessApprove;
                    grpChildAccessList.Add(grpchild);
                }
            }
            return grpChildAccessList;
        }


        public ActionResult fillCalendar(String MachineName = "")
        {
            var lstExeData = ExecutionScheduleList();
            return Json(lstExeData, JsonRequestBehavior.AllowGet);
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
                                  AccessCreate = GCC.GC.access_Create,
                                  AccessModify = GCC.GC.access_Modify,
                                  AccessDelete = GCC.GC.access_Delete,
                                  AccessView = GCC.GC.access_View,
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
                //var endDate = DateTime.Today.AddDays(15);
                // var data = scheModel.ScheduleMachines.Where(x => x.ApprovedDate == endDate).ToList();
                return Json(JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }

        }

        public List<Machine> ReviewerInstrumentList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<Machine> review = new List<Machine>();
                var Query1 = scheModel.tblMachineCreations
                   .Join(
                    scheModel.tblWorkFlowChilds,
                    M => M.WfMovedStep,
                    WC => WC.WFChild_Id,
                    (M, WC) => new
                    {
                        machineID = M.Machine_Id,
                        WorkflowStep = WC.FlowStep,
                        EmpID = WC.EmpId
                    }
                    ).ToList();

                var Query2 = scheModel.tblMachineCreations
                  .Join(
                   scheModel.tblWorkFlowChilds,
                   M => M.WfMovedStep,
                   WC => WC.WFChild_Id,
                   (M, WC) => new { M, WC })
                   .Join(
                   scheModel.tblAssignEmployeeGroups,
                   A => A.WC.GrpRoleId,
                   E => E.group_Id,
                   (A, E) => new
                   {
                       machineID = A.M.Machine_Id,
                       WorkflowStep = A.WC.FlowStep,
                       EmpID = E.employee_Id
                   }
                   ).ToList();

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
                                                     StatusId = MWF.M.Instru_Equip_StatusId

                                                 }).Where(y => y.machineStatus == "Reviewer" && y.StatusId == 1).ToList();
                Machine NewData;
                foreach (var item in machine)
                {
                    var Reviewer = Query1.Union(Query2).Where(x => x.machineID == item.machine_Id).ToList();

                    foreach (var Re in Reviewer)
                    {
                        if (item.machine_Id == Re.machineID && Re.WorkflowStep == "Reviewer" && Re.EmpID == SessionData.Employee_Id)
                        {
                            NewData = new Machine();
                            NewData.Machine_Id = item.machine_Id;
                            NewData.MachineID = item.machineId;
                            NewData.Machine_Name = item.machineName;
                            NewData.DepartmentName = item.machineDepartment;
                            NewData.Machine_Location = item.machineLocation;
                            NewData.WorkflowName = item.machineWorkflow;
                            NewData.Status = item.machineStatus;
                            review.Add(NewData);
                        }
                    }
                }
                return review;
            }
            catch (Exception e) { throw; }
        }
        public List<Machine> ReviewerEquipmentList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<Machine> review = new List<Machine>();
                var Query1 = scheModel.tblMachineCreations
                   .Join(
                    scheModel.tblWorkFlowChilds,
                    M => M.WfMovedStep,
                    WC => WC.WFChild_Id,
                    (M, WC) => new
                    {
                        machineID = M.Machine_Id,
                        WorkflowStep = WC.FlowStep,
                        EmpID = WC.EmpId
                    }
                    ).ToList();

                var Query2 = scheModel.tblMachineCreations
                  .Join(
                   scheModel.tblWorkFlowChilds,
                   M => M.WfMovedStep,
                   WC => WC.WFChild_Id,
                   (M, WC) => new { M, WC })
                   .Join(
                   scheModel.tblAssignEmployeeGroups,
                   A => A.WC.GrpRoleId,
                   E => E.group_Id,
                   (A, E) => new
                   {
                       machineID = A.M.Machine_Id,
                       WorkflowStep = A.WC.FlowStep,
                       EmpID = E.employee_Id
                   }
                   ).ToList();

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
                                                     StatusId = MWF.M.Instru_Equip_StatusId

                                                 }).Where(y => y.machineStatus == "Reviewer" && y.StatusId == 2).ToList();
                Machine NewData;
                foreach (var item in machine)
                {
                    var Reviewer = Query1.Union(Query2).Where(x => x.machineID == item.machine_Id).ToList();

                    foreach (var Re in Reviewer)
                    {
                        if (item.machine_Id == Re.machineID && Re.WorkflowStep == "Reviewer" && Re.EmpID == SessionData.Employee_Id)
                        {
                            NewData = new Machine();
                            NewData.Machine_Id = item.machine_Id;
                            NewData.MachineID = item.machineId;
                            NewData.Machine_Name = item.machineName;
                            NewData.DepartmentName = item.machineDepartment;
                            NewData.Machine_Location = item.machineLocation;
                            NewData.WorkflowName = item.machineWorkflow;
                            NewData.Status = item.machineStatus;
                            review.Add(NewData);
                        }
                    }
                }
                return review;
            }
            catch (Exception e) { throw; }
        }

        public List<Machine> ApproverInstrumentList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<Machine> Approved = new List<Machine>();
                var Query1 = scheModel.tblMachineCreations
                    .Join(
                     scheModel.tblWorkFlowChilds,
                     M => M.WfMovedStep,
                     WC => WC.WFChild_Id,
                     (M, WC) => new
                     {
                         machineID = M.Machine_Id,
                         WorkflowStep = WC.FlowStep,
                         EmpID = WC.EmpId
                     }
                     ).ToList();

                var Query2 = scheModel.tblMachineCreations
                  .Join(
                   scheModel.tblWorkFlowChilds,
                   M => M.WfMovedStep,
                   WC => WC.WFChild_Id,
                   (M, WC) => new { M, WC })
                   .Join(
                   scheModel.tblAssignEmployeeGroups,
                   A => A.WC.GrpRoleId,
                   E => E.group_Id,
                   (A, E) => new
                   {
                       machineID = A.M.Machine_Id,
                       WorkflowStep = A.WC.FlowStep,
                       EmpID = E.employee_Id
                   }
                   ).ToList();

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
                                                     StatusId = MWF.M.Instru_Equip_StatusId

                                                 }).Where(y => y.machineStatus == "Approver" && y.StatusId == 1).ToList();
                Machine NewData;
                foreach (var item in machine)
                {
                    var Approver = Query1.Union(Query2).Where(x => x.machineID == item.machine_Id).ToList();

                    foreach (var Ap in Approver)
                    {
                        if (item.machine_Id == Ap.machineID && Ap.WorkflowStep == "Approver" && Ap.EmpID == SessionData.Employee_Id)
                        {
                            NewData = new Machine();
                            NewData.Machine_Id = item.machine_Id;
                            NewData.MachineID = item.machineId;
                            NewData.Machine_Name = item.machineName;
                            NewData.DepartmentName = item.machineDepartment;
                            NewData.Machine_Location = item.machineLocation;
                            NewData.WorkflowName = item.machineWorkflow;
                            NewData.Status = item.machineStatus;
                            Approved.Add(NewData);
                        }
                    }
                }
                return Approved;
            }
            catch (Exception e) { throw; }
        }

        public List<Machine> ApproverEquipmentList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<Machine> Approved = new List<Machine>();
                var Query1 = scheModel.tblMachineCreations
                    .Join(
                     scheModel.tblWorkFlowChilds,
                     M => M.WfMovedStep,
                     WC => WC.WFChild_Id,
                     (M, WC) => new
                     {
                         machineID = M.Machine_Id,
                         WorkflowStep = WC.FlowStep,
                         EmpID = WC.EmpId
                     }
                     ).ToList();

                var Query2 = scheModel.tblMachineCreations
                  .Join(
                   scheModel.tblWorkFlowChilds,
                   M => M.WfMovedStep,
                   WC => WC.WFChild_Id,
                   (M, WC) => new { M, WC })
                   .Join(
                   scheModel.tblAssignEmployeeGroups,
                   A => A.WC.GrpRoleId,
                   E => E.group_Id,
                   (A, E) => new
                   {
                       machineID = A.M.Machine_Id,
                       WorkflowStep = A.WC.FlowStep,
                       EmpID = E.employee_Id
                   }
                   ).ToList();

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
                                                     StatusId = MWF.M.Instru_Equip_StatusId

                                                 }).Where(y => y.machineStatus == "Approver" && y.StatusId == 2).ToList();
                Machine NewData;
                foreach (var item in machine)
                {
                    var Approver = Query1.Union(Query2).Where(x => x.machineID == item.machine_Id).ToList();

                    foreach (var Ap in Approver)
                    {
                        if (item.machine_Id == Ap.machineID && Ap.WorkflowStep == "Approver" && Ap.EmpID == SessionData.Employee_Id)
                        {
                            NewData = new Machine();
                            NewData.Machine_Id = item.machine_Id;
                            NewData.MachineID = item.machineId;
                            NewData.Machine_Name = item.machineName;
                            NewData.DepartmentName = item.machineDepartment;
                            NewData.Machine_Location = item.machineLocation;
                            NewData.WorkflowName = item.machineWorkflow;
                            NewData.Status = item.machineStatus;
                            Approved.Add(NewData);
                        }
                    }
                }
                return Approved;
            }
            catch (Exception e) { throw; }
        }

        private tblMachineCreation oldMachinedata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.tblMachineCreations.Single(p => p.Machine_Id == id);
        }

        public ActionResult MachineApprove(long ID, string Remark)
        {
            try
            {
                var Result = 0;
                tblMachineCreation oldItem = oldMachinedata(ID);
                tblMachineCreation machine = new tblMachineCreation();

                machine.WfMovedStep = oldItem.WfMovedStep + 1;
                dbScheduleModel oldWF = new dbScheduleModel();
                var oldWFStep = oldWF.tblWorkFlowChilds.Where(x => x.WFChild_Id == oldItem.WfMovedStep).FirstOrDefault();
                var wfstepUpdated = scheModel.tblWorkFlowChilds.Where(x => x.WFChild_Id == machine.WfMovedStep && x.WorFlowId == oldItem.Machine_Workflow).FirstOrDefault();

                if (wfstepUpdated != null)
                {
                    if (wfstepUpdated.WorFlowId == oldItem.Machine_Workflow)
                    {
                        machine.Machine_Id = ID;
                        machine.Status = wfstepUpdated.FlowStep;
                        scheModel.tblMachineCreations.Attach(machine);
                        scheModel.Entry(machine).Property(x => x.WfMovedStep).IsModified = true;
                        scheModel.Entry(machine).Property(x => x.Status).IsModified = true;
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.Status.ToString(), machine.Status.ToString(), Remark);
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                        scheModel.SaveChanges();
                        Result = 1;
                    }
                }
                else
                {
                    machine.Status = "Active";
                    machine.Machine_Id = ID;
                    scheModel.tblMachineCreations.Attach(machine);
                    scheModel.Entry(machine).Property(x => x.Status).IsModified = true;
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.Status.ToString(), machine.Status.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                    scheModel.SaveChanges();
                    Result = 1;
                }


                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }
        }

        public ActionResult MachineReject(long ID, string Remark)
        {
            try
            {
                tblMachineCreation oldItem = oldMachinedata(ID);
                tblMachineCreation machine = new tblMachineCreation();

                dbScheduleModel oldWF = new dbScheduleModel();
                var oldWFStep = oldWF.tblWorkFlowChilds.Where(x => x.WFChild_Id == oldItem.WfMovedStep).FirstOrDefault();
                machine.Machine_Id = ID;
                machine.Status = "Rejected";
                machine.Remark = Remark;
                machine.WfMovedStep = oldWFStep.WFChild_Id;
                scheModel.tblMachineCreations.Attach(machine);
                scheModel.Entry(machine).Property(x => x.WfMovedStep).IsModified = true;
                scheModel.Entry(machine).Property(x => x.Status).IsModified = true;
                scheModel.Entry(machine).Property(x => x.Remark).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.Status.ToString(), machine.Status, Remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                scheModel.SaveChanges();
                var Result = 1;

                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }
        }

        public List<ScheduleMachine> ReviewerScheduleList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<ScheduleMachine> scheduleList = new List<ScheduleMachine>();

                var Query1 = scheModel.ScheduleMachines
                   .Join(
                    scheModel.tblMachineCreations,
                    S => S.MachineId,
                    M => M.Machine_Id,
                    (S, M) => new { S, M })
                    .Join(
                    scheModel.tblWorkFlowChilds,
                    SM => SM.S.ScheduleMovedStep,
                    WC => WC.WFChild_Id,
                    (SM, WC) => new
                    {
                        ScheduleId = SM.S.Id,
                        machineID = SM.S.MachineId,
                        WorkflowStep = WC.FlowStep,
                        EmpID = WC.EmpId
                    }
                    ).ToList();

                var Query2 = scheModel.ScheduleMachines
                   .Join(
                    scheModel.tblMachineCreations,
                    S => S.MachineId,
                    M => M.Machine_Id,
                    (S, M) => new { S, M })
                    .Join(
                    scheModel.tblWorkFlowChilds,
                    SM => SM.S.ScheduleMovedStep,
                    WC => WC.WFChild_Id,
                    (SM, WC) => new { SM, WC })
                    .Join(
                    scheModel.tblAssignEmployeeGroups,
                    A => A.WC.GrpRoleId,
                    E => E.group_Id,
                    (A, E) => new
                    {
                        ScheduleId = A.SM.S.Id,
                        machineID = A.SM.S.MachineId,
                        WorkflowStep = A.WC.FlowStep,
                        EmpID = E.employee_Id
                    }
                    ).ToList();
                var schedule = scheModel.ScheduleMachines.Where(x => x.Status == "Reviewer").ToList();
                foreach (var item in schedule)
                {
                    DateTime Date = DateTime.Now;
                    DateTime BGDate = Convert.ToDateTime(item.BeforegracedDate);
                    DateTime AGDate = Convert.ToDateTime(item.AftergracedDate);

                    if (Date >= BGDate && Date <= AGDate)
                    {
                        var Reviewer = Query1.Union(Query2).Where(x => x.ScheduleId == item.Id).ToList();

                        foreach (var Re in Reviewer)
                        {
                            if (item.MachineId == Re.machineID && Re.WorkflowStep == "Reviewer" && Re.EmpID == SessionData.Employee_Id)
                            {
                                var machine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == item.MachineId).FirstOrDefault();
                                item.FrequencyType = machine.MachineID;
                                scheduleList.Add(item);
                            }
                        }
                    }
                }
                return scheduleList;
            }
            catch (Exception e) { throw; }
        }

        public List<ScheduleMachine> ApproverScheduleList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<ScheduleMachine> scheduleList = new List<ScheduleMachine>();


                var Query1 = scheModel.ScheduleMachines
                   .Join(
                    scheModel.tblMachineCreations,
                    S => S.MachineId,
                    M => M.Machine_Id,
                    (S, M) => new { S, M })
                    .Join(
                    scheModel.tblWorkFlowChilds,
                    SM => SM.S.ScheduleMovedStep,
                    WC => WC.WFChild_Id,
                    (SM, WC) => new
                    {
                        ScheduleId = SM.S.Id,
                        machineID = SM.S.MachineId,
                        WorkflowStep = WC.FlowStep,
                        EmpID = WC.EmpId
                    }
                    ).ToList();

                var Query2 = scheModel.ScheduleMachines
                   .Join(
                    scheModel.tblMachineCreations,
                    S => S.MachineId,
                    M => M.Machine_Id,
                    (S, M) => new { S, M })
                    .Join(
                    scheModel.tblWorkFlowChilds,
                    SM => SM.S.ScheduleMovedStep,
                    WC => WC.WFChild_Id,
                    (SM, WC) => new { SM, WC })
                    .Join(
                    scheModel.tblAssignEmployeeGroups,
                    A => A.WC.GrpRoleId,
                    E => E.group_Id,
                    (A, E) => new
                    {
                        ScheduleId = A.SM.S.Id,
                        machineID = A.SM.S.MachineId,
                        WorkflowStep = A.WC.FlowStep,
                        EmpID = E.employee_Id
                    }
                    ).ToList();
                var schedule = scheModel.ScheduleMachines.Where(x => x.Status == "Approver").ToList();
                foreach (var item in schedule)
                {
                    DateTime Date = DateTime.Now;
                    DateTime BGDate = Convert.ToDateTime(item.BeforegracedDate);
                    DateTime AGDate = Convert.ToDateTime(item.AftergracedDate);
                    if (Date >= BGDate && Date <= AGDate)
                    {
                        var Approver = Query1.Union(Query2).Where(x => x.ScheduleId == item.Id).ToList();

                        foreach (var Ap in Approver)
                        {
                            if (item.MachineId == Ap.machineID && Ap.WorkflowStep == "Approver" && Ap.EmpID == SessionData.Employee_Id)
                            {
                                var machine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == item.MachineId).FirstOrDefault();
                                item.FrequencyType = machine.MachineID;
                                scheduleList.Add(item);
                            }
                        }
                    }
                }
                return scheduleList;
            }
            catch (Exception e) { throw; }
        }

        public List<ScheduleMachine> ExecutionScheduleList()
        {
            try
            {
                List<ScheduleMachine> List = new List<ScheduleMachine>();

                var schedule = scheModel.ScheduleMachines.Where(x => x.Status == "Execute").ToList();
                var ScheduleExe = scheModel.tblScheduleExecutions.ToList();
                foreach (var item in schedule)
                {
                    DateTime Date = DateTime.Now;
                    DateTime BGDate = Convert.ToDateTime(item.BeforegracedDate);
                    DateTime AGDate = Convert.ToDateTime(item.AftergracedDate);
                    if (Date >= BGDate && Date <= AGDate)
                    {
                        var machine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == item.MachineId).FirstOrDefault();
                        item.FrequencyType = machine.MachineID;
                        List.Add(item);
                    }

                }
                return List;
            }
            catch (Exception e) { throw; }
        }

        private ScheduleMachine oldScheduledata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.ScheduleMachines.Single(p => p.Id == id);
        }

        public ActionResult ScheduleApprove(long ID, string Remark)
        {
            try
            {
                var Result = 0;
                ScheduleMachine oldItem = oldScheduledata(ID);
                ScheduleMachine Schedule = new ScheduleMachine();
                long machineID = Convert.ToInt64(oldItem.MachineId);
                tblMachineCreation OldMachineWorkFlow = oldMachinedata(machineID);
                Schedule.ScheduleMovedStep = oldItem.ScheduleMovedStep + 1;
                dbScheduleModel oldWF = new dbScheduleModel();
                var oldWFStep = oldWF.tblWorkFlowChilds.Where(x => x.WFChild_Id == oldItem.ScheduleMovedStep).FirstOrDefault();
                var wfstepUpdated = scheModel.tblWorkFlowChilds.Where(x => x.WFChild_Id == Schedule.ScheduleMovedStep && x.WorFlowId == OldMachineWorkFlow.Machine_Workflow).FirstOrDefault();

                if (wfstepUpdated != null)
                {
                    if (wfstepUpdated.WorFlowId == OldMachineWorkFlow.Machine_Workflow)
                    {
                        Schedule.Id = ID;
                        Schedule.Status = wfstepUpdated.FlowStep;
                        scheModel.ScheduleMachines.Attach(Schedule);
                        scheModel.Entry(Schedule).Property(x => x.ScheduleMovedStep).IsModified = true;
                        scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.Status.ToString(), Schedule.Status.ToString(), Remark);
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                        scheModel.SaveChanges();
                        Result = 1;
                    }
                }
                else
                {
                    Schedule.Id = ID;
                    Schedule.Status = "Execute";
                    scheModel.ScheduleMachines.Attach(Schedule);
                    scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.Status.ToString(), Schedule.Status.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                    scheModel.SaveChanges();

                    if (oldItem.Remark == "")
                    {
                        DateTime Tilldate = new DateTime();
                        ScheduleMachine SM = new ScheduleMachine();
                        var Count = 0;
                        if (oldItem.TillDate == "6 Months")
                            Tilldate = Convert.ToDateTime(oldItem.ScheduleDueDate).AddMonths(6);
                        else if (oldItem.TillDate == "1 Year")
                            Tilldate = Convert.ToDateTime(oldItem.ScheduleDueDate).AddYears(1);
                        else if (oldItem.TillDate == "2 Year")
                            Tilldate = Convert.ToDateTime(oldItem.ScheduleDueDate).AddYears(2);

                        int diff = Tilldate.Subtract(Convert.ToDateTime(oldItem.ScheduleDueDate)).Days;
                        if (oldItem.FrequencyType == "15 Days")
                            Count = diff / 15;
                        if (oldItem.FrequencyType == "1 month")
                            Count = diff / 30;
                        if (oldItem.FrequencyType == "3 months")
                            Count = diff / 90;
                        if (oldItem.FrequencyType == "6 months")
                            Count = diff / 180;

                        DateTime date;
                        date = Convert.ToDateTime(oldItem.ScheduleDueDate);
                        SM.MachineId = oldItem.MachineId;
                        SM.ScheduleType = oldItem.ScheduleType;
                        SM.FrequencyType = oldItem.FrequencyType;
                        SM.InstallationDate = oldItem.InstallationDate;
                        SM.BGracedPeriod = oldItem.BGracedPeriod;
                        SM.AGrancedPeriod = oldItem.AGrancedPeriod;
                        SM.TillDate = oldItem.TillDate;
                        SM.Comment = oldItem.Comment;
                        SM.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                        SM.CreatedDepartment = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                        SM.ScheduleMovedStep = oldItem.ScheduleMovedStep;
                        SM.Status = "Execute";
                        for (int i = 0; i < Count; i++)
                        {
                            if (oldItem.FrequencyType == "15 Days")
                                date = date.AddDays(15);
                            if (oldItem.FrequencyType == "1 month")
                                date = date.AddMonths(1);
                            if (oldItem.FrequencyType == "3 months")
                                date = date.AddMonths(3);
                            if (oldItem.FrequencyType == "6 months")
                                date = date.AddMonths(6);
                            SM.ScheduleDueDate = date;
                            SM.BeforegracedDate = date.Subtract(TimeSpan.FromDays(Convert.ToInt64(SM.BGracedPeriod)));
                            SM.AftergracedDate = date.AddDays(Convert.ToInt64(SM.AGrancedPeriod));
                            SM.CreatedDate = DateTime.Now;
                            scheModel.ScheduleMachines.Add(SM);
                            scheModel.SaveChanges();
                        }
                    }

                    Result = 1;
                }


                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }
        }

        public ActionResult ScheduleReject(long ID, string Remark)
        {
            try
            {
                ScheduleMachine oldItem = oldScheduledata(ID);
                ScheduleMachine Schedule = new ScheduleMachine();

                dbScheduleModel oldWF = new dbScheduleModel();
                var oldWFStep = oldWF.tblWorkFlowChilds.Where(x => x.WFChild_Id == oldItem.ScheduleMovedStep).FirstOrDefault();
                Schedule.Id = ID;
                Schedule.Status = "Rejected";
                Schedule.Remark = Remark;
                scheModel.ScheduleMachines.Attach(Schedule);
                scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                scheModel.Entry(Schedule).Property(x => x.Remark).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.Status.ToString(), Schedule.Status.ToString(), Remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                scheModel.SaveChanges();
                var Result = 1;

                return Json(Result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }
        }

        public ActionResult ScheduleEdit(long id)
        {
            try
            {
                var lstSchedule = scheModel.ScheduleMachines.
                    Join(scheModel.tblMachineCreations,
                         S => S.MachineId, M => M.Machine_Id, (S, M) => new
                         {
                             id = S.Id,
                             machineID = M.MachineID,
                             SType = S.ScheduleType,
                             FType = S.FrequencyType,
                             InstDate = S.InstallationDate,
                             SDueDate = S.ScheduleDueDate,
                             BGP = S.BGracedPeriod,
                             AGP = S.AGrancedPeriod,
                             tillDate = S.TillDate,
                         }).Where(x => x.id == id).FirstOrDefault();
                return Json(lstSchedule, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public ActionResult ScheduleExecution(tblScheduleExecution ScheduleExe, string Remark, string RscheduleDate, string Reason)
        {
            try
            {

                ScheduleExe.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                ScheduleExe.CreatedDate = DateTime.Now;
                if (ScheduleExe.Schedule_Report_File != null)
                    ScheduleExe.Schedule_Report_File = Base64Encode(ScheduleExe.Schedule_Report_File);
                scheModel.tblScheduleExecutions.Add(ScheduleExe);
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], ScheduleExe.ScheduleExe_Id, "ScheduleExecution", "NA", "Schedule Execution", Remark);

                if (RscheduleDate != "")
                {
                    ScheduleMachine olditem = oldScheduledata(Convert.ToInt64(ScheduleExe.ScheduleId));
                    ScheduleMachine Schedule = new ScheduleMachine();
                    Schedule.Id = olditem.Id;
                    Schedule.Status = "ReSchedule";
                    scheModel.ScheduleMachines.Attach(Schedule);
                    scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.Status.ToString(), Schedule.Status.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                    scheModel.SaveChanges();

                    ScheduleMachine SM = new ScheduleMachine();
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
                    ).Where(x => x.MachineId == olditem.MachineId).ToList();
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
                        SM.Status = "Execute";
                    else
                        SM.Status = wfstepUpdated.FlowStep;

                    DateTime date;
                    SM.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                    SM.CreatedDepartment = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                    date = Convert.ToDateTime(RscheduleDate);
                    SM.BeforegracedDate = date.Subtract(TimeSpan.FromDays(Convert.ToInt64(olditem.BGracedPeriod)));
                    SM.AftergracedDate = date.AddDays(Convert.ToInt64(olditem.AGrancedPeriod));
                    SM.ScheduleDueDate = date;
                    SM.ScheduleType = olditem.ScheduleType;
                    SM.FrequencyType = olditem.FrequencyType;
                    SM.InstallationDate = olditem.InstallationDate;
                    SM.BGracedPeriod = olditem.BGracedPeriod;
                    SM.AGrancedPeriod = olditem.AGrancedPeriod;
                    SM.TillDate = olditem.TillDate;
                    SM.Comment = olditem.Comment;
                    SM.CreatedDate = DateTime.Now;
                    SM.MachineId = olditem.MachineId;
                    SM.OldScheduleID = olditem.Id;
                    SM.Remark = Reason;
                    scheModel.ScheduleMachines.Add(SM);
                    scheModel.SaveChanges();

                    if (olditem.ScheduleDueDate != SM.ScheduleDueDate)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.ScheduleDueDate.ToString(), SM.ScheduleDueDate.ToString(), Remark);
                    if (olditem.CreatedDepartment != SM.CreatedDepartment)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDepartment.ToString(), SM.CreatedDepartment.ToString(), Remark);
                    if (olditem.CreatedBy != SM.CreatedBy)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedBy.ToString(), SM.CreatedBy.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);


                }
                else if (ScheduleExe.Schedule_Status == "Skip" && RscheduleDate == null)
                {
                    ScheduleMachine olditem = oldScheduledata(Convert.ToInt64(ScheduleExe.ScheduleId));
                    ScheduleMachine Schedule = new ScheduleMachine();
                    Schedule.Id = olditem.Id;
                    Schedule.Status = "Skip";
                    scheModel.ScheduleMachines.Attach(Schedule);
                    scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.Status.ToString(), Schedule.Status.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                    scheModel.SaveChanges();

                }
                else
                {
                    ScheduleMachine olditem = oldScheduledata(Convert.ToInt64(ScheduleExe.ScheduleId));
                    ScheduleMachine Schedule = new ScheduleMachine();
                    Schedule.Id = olditem.Id;
                    Schedule.Status = "Completed";
                    scheModel.ScheduleMachines.Attach(Schedule);
                    scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.Status.ToString(), Schedule.Status.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                    scheModel.SaveChanges();

                }
                return Json("Save Success", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }

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