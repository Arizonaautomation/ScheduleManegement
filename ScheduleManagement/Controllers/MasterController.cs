using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.Models;
using TrainningManagement.SessionValidate;
using TrainningManagement.CommonClass;
using System.Data.Entity.Validation;
using System.Text;

namespace TrainningManagement.Controllers
{
    #region
    [ActionAuthorize]
    #endregion

    public class MasterController : Controller
    {
        // GET: Master
        dbScheduleModel scheModel = new dbScheduleModel();
        AuditTrail at = new AuditTrail();
        public ActionResult Index()
        {
            ViewBag.LstDpart = lstDprt();
            ViewBag.LstDesig = lstDsgnat();
            ViewBag.lstMachine = lstMachine();
            ViewBag.Employee = lstEmployee();
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
        public ActionResult EmployeeCreation()
        {
            ViewBag.Employee = lstEmployee();
            ViewBag.Groupchild = grpchildList();
            ViewBag.Department = fillmachineDropDown();
            ViewBag.Designation = fillEmpDropDown();
            return View();
        }

        public ActionResult HeadList(long departmentid, string Hd)
        {
            var HeadList = scheModel.tblEmployees.Where(x => x.HeadType == Hd && x.Department_UserType == "Head" && x.Employee_Department == departmentid).ToList();
            return Json(new { data = HeadList }, JsonRequestBehavior.AllowGet);
        }
        public List<tblDesignation> fillEmpDropDown()
        {
            List<tblDesignation> tbldes = new List<tblDesignation>();
            var lstdesig = scheModel.tblDesignations.ToList();
            foreach (var item in lstdesig)
            {
                tbldes.Add(new tblDesignation
                {
                    Designation_Id = item.Designation_Id,
                    Designation_Name = item.Designation_Name
                });
            }
            return tbldes;
        }
        public string ValidateEmailId(string emailId)
        {
            try
            {
                var emailStatus = scheModel.tblEmployees.Where(m => m.Employee_Email == emailId).FirstOrDefault();
                if (emailStatus != null)
                {
                    return "1";
                }
                else
                {
                    return "0";
                }
            }
            catch (Exception e) { throw; }
        }
        private tblEmployee oldemployeedata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.tblEmployees.Single(p => p.Employee_Id == id);
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }
        public ActionResult EmpCreation(tblEmployee empCreation)
        {

            try
            {
                empCreation.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Name;
                var isExistm = scheModel.tblEmployees.Any(x => x.Employee_Id == empCreation.Employee_Id);
                if (!isExistm)
                {
                    empCreation.EmpProfilePhoto = Base64Encode(empCreation.EmpProfilePhoto);
                    empCreation.CreateDate = DateTime.Now;
                    empCreation.FirstTimeLoginStatus = "FALSE";
                    scheModel.tblEmployees.Add(empCreation);
                    scheModel.SaveChanges();
                    string[] grpId = empCreation.grpid.Split(',');
                    foreach (var item in grpId)
                    {
                        tblAssignEmployeeGroup grp = new tblAssignEmployeeGroup();
                        grp.employee_Id = empCreation.Employee_Id;
                        grp.group_Id = Convert.ToInt64(item);
                        scheModel.tblAssignEmployeeGroups.Add(grp);
                        scheModel.SaveChanges();
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", "NA", "New Employee Created", empCreation.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(empCreation).State = EntityState.Modified;
                    tblEmployee olditem = oldemployeedata(empCreation.Employee_Id);
                    scheModel.tblEmployees.Attach(empCreation);

                    if (olditem.Employee_Department != empCreation.Employee_Department)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Department.ToString(), empCreation.Employee_Department.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Department).IsModified = true;
                    }
                    if (olditem.Employee_Designation != empCreation.Employee_Designation)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Designation.ToString(), empCreation.Employee_Designation.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Designation).IsModified = true;
                    }
                    if (olditem.Employee_Name != empCreation.Employee_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Name.ToString(), empCreation.Employee_Name.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Name).IsModified = true;
                    }
                    if (olditem.Employee_Email != empCreation.Employee_Email)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Email, empCreation.Employee_Email, empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Email).IsModified = true;
                    }
                    if (olditem.Employee_Contact != empCreation.Employee_Contact)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Contact, empCreation.Employee_Contact, empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Contact).IsModified = true;
                    }
                    if (olditem.Employee_Password != empCreation.Employee_Password)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Password.ToString(), empCreation.Employee_Password.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Password).IsModified = true;
                    }
                    if (olditem.Employee_Type != empCreation.Employee_Type)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Type.ToString(), empCreation.Employee_Type.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Type).IsModified = true;
                    }
                    if (olditem.group_Id != empCreation.group_Id)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.group_Id.ToString(), empCreation.group_Id.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.group_Id).IsModified = true;
                    }
                    if (olditem.Employee_Address != empCreation.Employee_Address)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Address.ToString(), empCreation.Employee_Address.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Address).IsModified = true;
                    }
                    if (olditem.CreatedBy != empCreation.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.CreatedBy.ToString(), empCreation.CreatedBy.ToString(), empCreation.Remark);
                        scheModel.Entry(empCreation).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.CreateDate.ToString(), DateTime.Now.ToString(), empCreation.Remark);
                    scheModel.SaveChanges();

                    string[] grpId = empCreation.grpid.Split(',');
                    foreach (var item in grpId)
                    {
                        tblAssignEmployeeGroup grp = new tblAssignEmployeeGroup();
                        grp.employee_Id = empCreation.Employee_Id;
                        grp.group_Id = Convert.ToInt64(item);
                        scheModel.tblAssignEmployeeGroups.Add(grp);
                        scheModel.SaveChanges();
                    }

                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", " NA", "NA", ex.Message);
                throw;
            }

            //catch (DbEntityValidationException e)
            //{
            //    foreach (var eve in e.EntityValidationErrors)
            //    {
            //        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
            //            eve.Entry.Entity.GetType().Name, eve.Entry.State);
            //        foreach (var ve in eve.ValidationErrors)
            //        {
            //            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
            //                ve.PropertyName, ve.ErrorMessage);
            //        }
            //    }
            //    throw;
            //}
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public ActionResult EmployeeEdit(long id)
        {
            try
            {
                var lstEmp = scheModel.tblEmployees.Where(x => x.Employee_Id == id).FirstOrDefault();
                lstEmp.EmpProfilePhoto = Base64Decode(lstEmp.EmpProfilePhoto);
                return Json(lstEmp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult EmployeeDelete(long Id, string remark)
        {
            var lstEmp = scheModel.tblEmployees.Where(x => x.Employee_Id == Id).FirstOrDefault();
            try
            {
                scheModel.Entry(lstEmp).State = EntityState.Deleted;
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstEmp.Employee_Id, "Employee", "Created Employee", "Delete Data", remark);
                return Json("Data Delete Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstEmp.Employee_Id, "Employee", "Created Employee", "Delete Data", ex.Message);
                throw;
            }
        }
        public JsonResult ListEmployeeType()
        {
            try
            {
                List<string> EmployeeTypeList = new List<string>();
                EmployeeTypeList.Add("Admin");
                EmployeeTypeList.Add("User");
                EmployeeTypeList.Add("Reviewer");
                EmployeeTypeList.Add("Approver");
                return Json(new { data = EmployeeTypeList }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public JsonResult lstGroup()
        {
            try
            {
                List<tblAccessGroupMaster> lstGroup = new List<tblAccessGroupMaster>();

                var grp = scheModel.tblAccessGroupMasters
                    .Join(
                    scheModel.tblAccessGroupChilds,
                    GM => GM.group_Id,
                    GC => GC.group_Id,
                  (GM, GC) => new
                  {
                      GroupID = GM.group_Id,
                      GroupName = GM.group_Name
                  }).Distinct().ToList();
                foreach (var item in grp)
                {
                    tblAccessGroupMaster gm = new tblAccessGroupMaster();
                    gm.group_Id = item.GroupID;
                    gm.group_Name = item.GroupName;
                    gm.remark = "";
                    lstGroup.Add(gm);
                }
                return Json(new { data = lstGroup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<tblEmployee> lstEmployee()
        {
            try
            {
                var sessionData = ((tblEmployee)(Session["EmployeeData"]));
                List<tblEmployee> lstEmployee = new List<tblEmployee>();
                var emp = scheModel.tblEmployees.Where(y => y.Employee_Department == sessionData.Employee_Department).ToList();
                for (int i = 0; i < emp.Count; i++)
                {
                    lstEmployee.Add(emp[i]);
                }
                return lstEmployee;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #region Department
        public ActionResult Department()
        {
            ViewBag.LstDpart = lstDprt();
            ViewBag.Groupchild = grpchildList();
            return View();
        }

        private tblDepartment oldDepartmentdata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.tblDepartments.Single(p => p.Department_Id == id);
        }

        public ActionResult DepartmentCreation(tblDepartment deprtment)
        {
            try
            {
                deprtment.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                var isExistm = scheModel.tblDepartments.Any(x => x.Department_Id == deprtment.Department_Id);
                if (!isExistm)
                {
                    deprtment.CreatedDate = DateTime.Now;
                    scheModel.tblDepartments.Add(deprtment);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], deprtment.Department_Id, "Department", "NA", "New Department Created", deprtment.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // scheModel.Entry(deprtment).State = EntityState.Modified;
                    tblDepartment oldItem = oldDepartmentdata(deprtment.Department_Id);
                    scheModel.tblDepartments.Attach(deprtment);

                    if (oldItem.Department_Name != deprtment.Department_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.Department_Name, deprtment.Department_Name, deprtment.Remark);
                        scheModel.Entry(deprtment).Property(x => x.Department_Name).IsModified = true;
                    }
                    if (oldItem.Department_Description != deprtment.Department_Description)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.Department_Description, deprtment.Department_Description, deprtment.Remark);
                        scheModel.Entry(deprtment).Property(x => x.Department_Description).IsModified = true;
                    }
                    if (oldItem.CreatedBy != deprtment.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.CreatedBy.ToString(), deprtment.CreatedBy.ToString(), deprtment.Remark);
                        scheModel.Entry(deprtment).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), deprtment.Remark);
                    scheModel.SaveChanges();

                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], deprtment.Department_Id, "Department", "NA", "NA", ex.Message);
                throw;
            }
        }
        public ActionResult DepartmentEdit(long id)
        {
            try
            {
                var lstdepartment = scheModel.tblDepartments.Where(x => x.Department_Id == id).FirstOrDefault();
                return Json(lstdepartment, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult DepartmentDelete(long id, string remark)
        {
            try
            {
                var lstdepartment = scheModel.tblDepartments.Where(x => x.Department_Id == id).FirstOrDefault();
                scheModel.Entry(lstdepartment).State = EntityState.Deleted;
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstdepartment.Department_Id, "Department", "Created Department", "Delete Department", remark);
                return Json("Data Delete Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "Department", "Created Department", "Delete Department", ex.Message);
                throw;
            }
        }
        public List<tblDepartment> lstDprt()
        {
            try
            {
                List<tblDepartment> lstdepart = new List<tblDepartment>();
                var depart = scheModel.tblDepartments.ToList();
                for (int i = 0; i < depart.Count; i++)
                {
                    lstdepart.Add(depart[i]);
                }
                return lstdepart;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
        #region Designation
        public ActionResult Designation()
        {
            ViewBag.LstDesig = lstDsgnat();
            ViewBag.Groupchild = grpchildList();
            return View();
        }

        private tblDesignation oldDesignationdata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.tblDesignations.Single(p => p.Designation_Id == id);
        }

        public ActionResult DesignationCreation(tblDesignation designation)
        {
            try
            {
                designation.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                var isExistm = scheModel.tblDesignations.Any(x => x.Designation_Id == designation.Designation_Id);
                if (!isExistm)
                {
                    designation.CreatedDate = DateTime.Now;
                    scheModel.tblDesignations.Add(designation);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", "NA", "New Designation Created", designation.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(designation).State = EntityState.Modified;

                    tblDesignation oldItem = oldDesignationdata(designation.Designation_Id);
                    scheModel.tblDesignations.Attach(designation);
                    if (oldItem.Designation_Name != designation.Designation_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", oldItem.Designation_Name, designation.Designation_Name, designation.Remark);
                        scheModel.Entry(designation).Property(x => x.Designation_Name).IsModified = true;

                    }
                    if (oldItem.Designation_Description != designation.Designation_Description)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", oldItem.Designation_Description, designation.Designation_Description, designation.Remark);
                        scheModel.Entry(designation).Property(x => x.Designation_Description).IsModified = true;
                    }
                    if (oldItem.CreatedBy != designation.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Designation_Id, "Designation", oldItem.CreatedBy.ToString(), designation.CreatedBy.ToString(), designation.Remark);
                        scheModel.Entry(designation).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Designation_Id, "Designation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), designation.Remark);
                    scheModel.SaveChanges();
                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", "NA", "NA", ex.Message);
                throw;
            }
        }
        public ActionResult DesignationEdit(long id)
        {
            try
            {
                var lstDesignation = scheModel.tblDesignations.Where(x => x.Designation_Id == id).FirstOrDefault();
                return Json(lstDesignation, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult DesignationDelete(long id, string remark)
        {
            try
            {
                var lstDesignation = scheModel.tblDesignations.Where(x => x.Designation_Id == id).FirstOrDefault();
                scheModel.Entry(lstDesignation).State = EntityState.Deleted;
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstDesignation.Designation_Id, "Designation", "Created Designation", "Delete Designation", remark);
                return Json("Data Delete Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "Designation", "Created Designation", "Delete Designation", ex.Message);
                throw;
            }
        }
        public List<tblDesignation> lstDsgnat()
        {
            try
            {
                List<tblDesignation> lstDsgnat = new List<tblDesignation>();
                var desig = scheModel.tblDesignations.ToList();
                for (int i = 0; i < desig.Count; i++)
                {
                    lstDsgnat.Add(desig[i]);
                }
                return lstDsgnat;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion
        #region Machine
        public ActionResult Machine()
        {
            ViewBag.lstMachine = lstMachine();
            ViewBag.Groupchild = grpchildList();
            ViewBag.MachineDepartment = fillmachineDropDown();
            return View();
        }

        private tblMachineCreation oldMachinedata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.tblMachineCreations.Single(p => p.Machine_Id == id);
        }

        public ActionResult MachineCreation(tblMachineCreation machine)
        {
            try
            {
                machine.CreatedDate = DateTime.Now;
                machine.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                var isExistm = scheModel.tblMachineCreations.Any(x => x.Machine_Id == machine.Machine_Id);
                if (!isExistm)
                {
                    scheModel.tblMachineCreations.Add(machine);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], machine.Machine_Id, "MachineCreation", "NA", "New Machine Created", machine.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(machine).State = EntityState.Modified;
                    tblMachineCreation olditem = oldMachinedata(machine.Machine_Id);
                    scheModel.tblMachineCreations.Attach(machine);


                    if (olditem.Machine_Department != machine.Machine_Department)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Department.ToString(), machine.Machine_Department.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Department).IsModified = true;

                    }
                    if (olditem.Machine_Name != machine.Machine_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Name.ToString(), machine.Machine_Name.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Department).IsModified = true;
                    }
                    if (olditem.Machine_Location != machine.Machine_Location)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Location.ToString(), machine.Machine_Location.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Location).IsModified = true;
                    }
                    if (olditem.Machine_InstallationTime != machine.Machine_InstallationTime)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_InstallationTime.ToString(), machine.Machine_InstallationTime.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_InstallationTime).IsModified = true;
                    }
                    if (olditem.Machine_ScheduleType != machine.Machine_ScheduleType)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_ScheduleType.ToString(), machine.Machine_ScheduleType.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_ScheduleType).IsModified = true;
                    }
                    if (olditem.Calibration_Date != machine.Calibration_Date)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Calibration_Date.ToString(), machine.Calibration_Date.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.Calibration_Date).IsModified = true;
                    }
                    if (olditem.FirstDueCalibration_Date != machine.FirstDueCalibration_Date)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.FirstDueCalibration_Date.ToString(), machine.FirstDueCalibration_Date.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.FirstDueCalibration_Date).IsModified = true;
                    }
                    if (olditem.Status != machine.Status)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Status.ToString(), machine.Status.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.CreatedBy).IsModified = true;
                    }

                    if (olditem.CreatedBy != machine.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.CreatedBy.ToString(), machine.CreatedBy.ToString(), machine.Remark);
                        scheModel.Entry(machine).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), machine.Remark);

                    scheModel.SaveChanges();

                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], machine.Machine_Id, "MachineCreation", "NA", "NA", ex.Message);
                throw;
            }
        }
        public ActionResult MachineEdit(long id)
        {
            try
            {
                var lstmachine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == id).FirstOrDefault();
                return Json(lstmachine, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult MachineDelete(long id, string remark)
        {
            try
            {
                var lstlstmachine = scheModel.tblMachineCreations.Where(x => x.Machine_Id == id).FirstOrDefault();
                scheModel.Entry(lstlstmachine).State = EntityState.Deleted;
                scheModel.SaveChanges();
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstlstmachine.Machine_Id, "MachineCreation", "Created Machine", "Delete Machine", remark);
                return Json("Data Delete Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "MachineCreation", "Created Machine", "Delete Machine", ex.Message);
                throw;
            }
        }
        public List<tblMachineCreation> lstMachine()
        {
            try
            {
                List<tblMachineCreation> lstMachine = new List<tblMachineCreation>();
                var sessionData = ((tblEmployee)(Session["EmployeeData"]));
                var machine = scheModel.tblMachineCreations.Where(y => y.Machine_Department == sessionData.Employee_Department).ToList();
                for (int i = 0; i < machine.Count; i++)
                {
                    lstMachine.Add(machine[i]);
                }
                return lstMachine;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public List<tblDepartment> fillmachineDropDown()
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
        #endregion


    }




}

//public class employee
//{
//    public long empid { get; set; }
//    public long empdept { get; set; }
//    public long empdesig { get; set; }
//    public string empname { get; set; }
//    public string empemail { get; set; }
//    public string empcontact { get; set; }
//    public string emppassword { get; set; }
//    public string emptype { get; set; }
//    public string crtby { get; set; }
//    public Nullable<System.DateTime> crtdatetime { get; set; }
//    public Nullable<long> grpid { get; set; }
//    public string empaddress { get; set; }
//}