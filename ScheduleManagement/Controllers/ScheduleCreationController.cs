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
            ViewBag.ScheduleEndDate = lstTillDate();
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

        public List<tblScheduleEndType> lstTillDate()
        {
            List<tblScheduleEndType> lstScheduleType = new List<tblScheduleEndType>();

            lstScheduleType = scheModel.tblScheduleEndTypes.ToList();
            return lstScheduleType;
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
        public List<MachineSchedule> ShceduleList()
        {
            List<MachineSchedule> SList = new List<MachineSchedule>();
            var Schedule = scheModel.ScheduleMachines.
                Join(
                scheModel.tblMachineCreations,
                S => S.MachineId,
                M => M.Machine_Id,
                (S, M) => new
                {
                    id = S.Id,
                    MID = M.MachineID,
                    SType = S.ScheduleType,
                    FType = S.FrequencyType,
                    InsDate = S.InstallationDate,
                    SdueDate = S.ScheduleDueDate,
                    BGP = S.BGracedPeriod,
                    AGP = S.AGrancedPeriod,
                    tillDate = S.TillDate,
                    comment = S.Comment,
                    status = S.Status,
                    remark = S.Remark

                }
                ).Where(x => x.status == "Rejected").ToList();
            MachineSchedule MS;
            foreach (var item in Schedule)
            {
                MS = new MachineSchedule();
                MS.Id = item.id;
                MS.MachineID = item.MID;
                MS.ScheduleType = item.SType;
                MS.FrequencyType = item.FType;
                MS.InstallationDate = Convert.ToDateTime(item.InsDate);
                MS.ScheduleDueDate = Convert.ToDateTime(item.SdueDate);
                MS.BGracedPeriod = item.BGP;
                MS.AGrancedPeriod = item.AGP;
                MS.TillDate = item.tillDate;
                MS.Comment = item.comment;
                MS.Status = item.status;
                MS.Remark = item.remark;
                SList.Add(MS);
            }
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
                //var endDateID = Convert.ToInt64(SM.TillDate);
                //var enddaateType = scheModel.tblScheduleEndTypes.Where(x => x.scheduleEnd_Id == endDateID).FirstOrDefault();
                //SM.TillDate = enddaateType.ScheduleEnd_Type;

                var isExistm = scheModel.ScheduleMachines.Any(x => x.Id == SM.Id);
                if (!isExistm)
                {
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


                    DateTime date;
                    SM.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                    SM.CreatedDepartment = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                    date = Convert.ToDateTime(SM.ScheduleDueDate);
                    SM.BeforegracedDate = date.Subtract(TimeSpan.FromDays(Convert.ToInt64(SM.BGracedPeriod)));
                    SM.AftergracedDate = date.AddDays(Convert.ToInt64(SM.AGrancedPeriod));
                    SM.CreatedDate = DateTime.Now;
                    scheModel.ScheduleMachines.Add(SM);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], SM.Id, "ScheduleMachine", "NA", "New Schedule Created", SM.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    ScheduleMachine olditem = oldScheduledata(SM.Id);

                    if (olditem.Status == "Rejected")
                    {
                        ScheduleMachine oldS = new ScheduleMachine();
                        oldS.Id = SM.Id;
                        oldS.Status = "AfterReject";
                        scheModel.ScheduleMachines.Attach(oldS);
                        scheModel.Entry(oldS).Property(x => x.Status).IsModified = true;
                        scheModel.SaveChanges();

                    }

                    var MachineID = Convert.ToInt64(SM.otherdata.MachineID);
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
                    ).Where(x => x.MachineId == MachineID).ToList();
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

                    DateTime date;
                    SM.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                    SM.CreatedDepartment = ((tblEmployee)(Session["EmployeeData"])).Employee_Department;
                    date = Convert.ToDateTime(SM.ScheduleDueDate);
                    SM.BeforegracedDate = date.Subtract(TimeSpan.FromDays(Convert.ToInt64(SM.BGracedPeriod)));
                    SM.AftergracedDate = date.AddDays(Convert.ToInt64(SM.AGrancedPeriod));
                    SM.CreatedDate = DateTime.Now;
                    SM.MachineId = MachineID;
                    scheModel.ScheduleMachines.Add(SM);
                    scheModel.SaveChanges();

                    if (olditem.ScheduleType != SM.ScheduleType)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.ScheduleType.ToString(), SM.ScheduleType.ToString(), SM.otherdata.Remark);
                    if (olditem.FrequencyType != SM.FrequencyType)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.FrequencyType.ToString(), SM.FrequencyType.ToString(), SM.otherdata.Remark);
                    if (olditem.InstallationDate != SM.InstallationDate)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.InstallationDate.ToString(), SM.InstallationDate.ToString(), SM.otherdata.Remark);
                    if (olditem.ScheduleDueDate != SM.ScheduleDueDate)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.ScheduleDueDate.ToString(), SM.ScheduleDueDate.ToString(), SM.otherdata.Remark);
                    if (olditem.BGracedPeriod != SM.BGracedPeriod)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.BGracedPeriod.ToString(), SM.BGracedPeriod.ToString(), SM.otherdata.Remark);
                    if (olditem.AGrancedPeriod != SM.AGrancedPeriod)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.AGrancedPeriod.ToString(), SM.AGrancedPeriod.ToString(), SM.otherdata.Remark);
                    if (olditem.TillDate != SM.TillDate)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.TillDate.ToString(), SM.TillDate.ToString(), SM.otherdata.Remark);
                    if (olditem.Comment != SM.Comment)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.Comment.ToString(), SM.Comment.ToString(), SM.otherdata.Remark);
                    if (olditem.CreatedBy != SM.CreatedBy)
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedBy.ToString(), SM.CreatedBy.ToString(), SM.otherdata.Remark);
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Id, "ScheduleMachine", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), SM.otherdata.Remark);


                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], SM.Id, "ScheduleMachine", "NA", "New Schedule Created", SM.otherdata.Remark);
                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }
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
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], Id, "ScheduleMachine", "NA", "Deactive", ex.Message);
                throw;
            }
        }

    }

    public class MachineSchedule
    {
        public long Id { get; set; }
        public string MachineID { get; set; }
        public string ScheduleType { get; set; }
        public string FrequencyType { get; set; }
        public DateTime InstallationDate { get; set; }
        public DateTime ScheduleDueDate { get; set; }
        public string BGracedPeriod { get; set; }
        public string AGrancedPeriod { get; set; }
        public string TillDate { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }
}