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

    public class HomeController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();
        MasterController machine = new MasterController();
        AuditTrail at = new AuditTrail();
        public ActionResult Index()
        {
            Session["MenuList"] = MenuList();
            ViewBag.MachineReviewer = ReviewerMachineList();
            ViewBag.MachineApprover = ApproverMachineList();
            ViewBag.ScheduleReviewer = ReviewerScheduleList();
            ViewBag.ScheduleApprover = ApproverScheduleList();
            ViewBag.ScheduleExecution = ExecutionScheduleList();
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

        public List<Machine> ReviewerMachineList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<Machine> review = new List<Machine>();
                var Reviewer = scheModel.tblMachineCreations
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
                        EmpID = A.WC.EmpId,
                        GrpEmpID = E.employee_Id
                    }
                    ).Distinct().ToList();

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

                                                 }).Where(y => y.MachineDepId == SessionData.Employee_Department).ToList();
                Machine NewData;
                foreach (var item in machine)
                {
                    foreach (var Re in Reviewer)
                    {
                        if (item.machine_Id == Re.machineID && Re.WorkflowStep == "Reviewer" && (Re.EmpID == SessionData.Employee_Id || Re.GrpEmpID == SessionData.Employee_Id))
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

        public List<Machine> ApproverMachineList()
        {
            try
            {
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<Machine> Approved = new List<Machine>();
                var Approver = scheModel.tblMachineCreations
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
                        EmpID = A.WC.EmpId,
                        GrpEmpID = E.employee_Id
                    }
                    ).Distinct().ToList();
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

                                                 }).Where(y => y.MachineDepId == SessionData.Employee_Department).ToList();
                Machine NewData;
                foreach (var item in machine)
                {
                    foreach (var Ap in Approver)
                    {
                        if (item.machine_Id == Ap.machineID && Ap.WorkflowStep == "Approver" && (Ap.EmpID == SessionData.Employee_Id || Ap.GrpEmpID == SessionData.Employee_Id))
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
                var wfstepUpdated = scheModel.tblWorkFlowChilds.Where(x => x.WFChild_Id == machine.WfMovedStep && x.WorFlowId == machine.Machine_Workflow).FirstOrDefault();

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
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.Status.ToString(),machine.Status.ToString(), Remark);
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
                machine.Status ="Rejected";
                machine.Machine_Id = ID;
                scheModel.tblMachineCreations.Attach(machine);
                scheModel.Entry(machine).Property(x => x.WfMovedStep).IsModified = true;
                scheModel.Entry(machine).Property(x => x.Status).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "MachineCreation", oldItem.Status.ToString(),machine.Status, Remark);
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

                var Reviewer = scheModel.ScheduleMachines
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
                        machineID = A.SM.S.MachineId,
                        WorkflowStep = A.WC.FlowStep,
                        EmpID = A.WC.EmpId,
                        GrpEmpID = E.employee_Id
                    }
                    ).Distinct().ToList();

                var schedule = scheModel.ScheduleMachines.ToList();
                var show = 0;
                foreach (var item in schedule)
                {
                    foreach (var Re in Reviewer)
                    {
                        if (item.MachineId == Re.machineID && Re.WorkflowStep == "Reviewer" && (Re.EmpID == SessionData.Employee_Id || Re.GrpEmpID == SessionData.Employee_Id))
                        {
                            if (item.ApprovedRejectStatus == null)
                            {
                                show = 0;
                                if (scheduleList.Count != 0)
                                {
                                    foreach (var list in scheduleList)
                                    {
                                        if (item.MachineId != list.MachineId)
                                        {
                                            show = 1;
                                            break;
                                        }
                                    }
                                }
                                else
                                    show = 1;
                                if (show == 1)
                                {
                                    var machine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == item.MachineId).FirstOrDefault();
                                    item.FrequencyType = machine.MachineID;
                                    scheduleList.Add(item);
                                }
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

                var Approver = scheModel.ScheduleMachines
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
                machineID = A.SM.S.MachineId,
                WorkflowStep = A.WC.FlowStep,
                EmpID = A.WC.EmpId,
                GrpEmpID = E.employee_Id
            }
            ).Distinct().ToList();

                var schedule = scheModel.ScheduleMachines.ToList();
                var show = 0;
                foreach (var item in schedule)
                {
                    foreach (var Ap in Approver)
                    {
                        if (item.MachineId == Ap.machineID && Ap.WorkflowStep == "Approver" && (Ap.EmpID == SessionData.Employee_Id || Ap.GrpEmpID == SessionData.Employee_Id))
                        {
                            if (item.ApprovedRejectStatus == null)
                            {
                                show = 0;
                                if (scheduleList.Count != 0)
                                {
                                    foreach (var list in scheduleList)
                                    {
                                        if (item.MachineId != list.MachineId)
                                        {
                                            show = 1;
                                            break;
                                        }
                                    }
                                }
                                else
                                    show = 1;
                                if (show == 1)
                                {
                                    var machine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == item.MachineId).FirstOrDefault();
                                    item.FrequencyType = machine.MachineID;
                                    scheduleList.Add(item);
                                }
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

                var schedule = scheModel.ScheduleMachines.Where(x => x.ApprovedRejectStatus == 1).ToList();
                foreach (var item in schedule)
                {
                    var machine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == item.MachineId).FirstOrDefault();
                    item.FrequencyType = machine.MachineID;
                    List.Add(item);
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
                    Schedule.Status = "Completed";
                    scheModel.ScheduleMachines.Attach(Schedule);
                    scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.Status.ToString(), Schedule.Status.ToString(), Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                    scheModel.SaveChanges();
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
                scheModel.ScheduleMachines.Attach(Schedule);
                scheModel.Entry(Schedule).Property(x => x.Status).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.Status.ToString(), Schedule.Status.ToString(), Remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Id, "ScheduleMachine", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), Remark);
                scheModel.SaveChanges();
                var Result = 1;

                return Json(Result, JsonRequestBehavior.AllowGet);
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