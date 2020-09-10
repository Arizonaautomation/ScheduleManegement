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
        public ActionResult EmployeeCreation()
        {
            ViewBag.RoleList = lstRole();
            ViewBag.Employee = lstEmployee();
            ViewBag.Groupchild = grpchildList();
            ViewBag.Department = fillDepartmentDropDown();
            ViewBag.Designation = fillDesigationDropDown();
            return View();
        }

        //public ActionResult HeadList(long departmentid, string Hd)
        //{
        //    var HeadList = scheModel.tblEmployees.Where(x => x.HeadType == Hd && x.Department_UserType == "Head" && x.Employee_Department == departmentid).ToList();
        //    return Json(new { data = HeadList }, JsonRequestBehavior.AllowGet);
        //}

        #region Employee
        public List<tblDepartment> fillDepartmentDropDown()
        {
            var lstDprt = scheModel.tblDepartments.Where(x => x.Status == "Active").ToList();
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
        public List<tblDesignation> fillDesigationDropDown()
        {
            List<tblDesignation> tbldes = new List<tblDesignation>();
            var lstdesig = scheModel.tblDesignations.Where(x => x.Status == "Active").ToList();
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
        public string ValidateEmpId(string id)
        {
            try
            {
                var empIdStatus = scheModel.tblEmployees.Where(m => m.EmpID == id).FirstOrDefault();
                if (empIdStatus != null)
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
                    if (empCreation.EmpProfilePhoto != null)
                        empCreation.EmpProfilePhoto = Base64Encode(empCreation.EmpProfilePhoto);
                    empCreation.CreatedDate = DateTime.Now;
                    empCreation.Status = "Active";
                    empCreation.FirstTimeLoginStatus = "FALSE";
                    scheModel.tblEmployees.Add(empCreation);
                    scheModel.SaveChanges();
                    string[] grpId = empCreation.otherdata.grpid.Split(',');
                    foreach (var item in grpId)
                    {
                        tblAssignEmployeeGroup grp = new tblAssignEmployeeGroup();
                        grp.employee_Id = empCreation.Employee_Id;
                        grp.group_Id = Convert.ToInt64(item);
                        scheModel.tblAssignEmployeeGroups.Add(grp);
                        scheModel.SaveChanges();
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", "NA", "New Employee Created", empCreation.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(empCreation).State = EntityState.Modified;
                    tblEmployee olditem = oldemployeedata(empCreation.Employee_Id);
                    scheModel.tblEmployees.Attach(empCreation);
                    empCreation.Status = "Active";
                    if (olditem.Employee_Department != empCreation.Employee_Department)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Department.ToString(), empCreation.Employee_Department.ToString(), empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Department).IsModified = true;
                    }
                    if (olditem.Employee_Designation != empCreation.Employee_Designation)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Designation.ToString(), empCreation.Employee_Designation.ToString(), empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Designation).IsModified = true;
                    }
                    if (olditem.Employee_Name != empCreation.Employee_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Name, empCreation.Employee_Name, empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Name).IsModified = true;
                    }
                    if (olditem.Employee_Email != empCreation.Employee_Email)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Email, empCreation.Employee_Email, empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Email).IsModified = true;
                    }
                    if (olditem.Employee_Contact != empCreation.Employee_Contact)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Contact, empCreation.Employee_Contact, empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Contact).IsModified = true;
                    }
                    if (olditem.Employee_Password != empCreation.Employee_Password)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Password, empCreation.Employee_Password, empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Password).IsModified = true;
                    }
                    //if (olditem.Employee_Type != empCreation.Employee_Type)
                    //{
                    //    at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Type.ToString(), empCreation.Employee_Type.ToString(), empCreation.otherdata.Remark);
                    //    scheModel.Entry(empCreation).Property(x => x.Employee_Type).IsModified = true;
                    //}
                    if (olditem.group_Id != empCreation.group_Id)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.group_Id.ToString(), empCreation.group_Id.ToString(), empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.group_Id).IsModified = true;
                    }
                    if (olditem.Employee_Address != empCreation.Employee_Address)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.Employee_Address, empCreation.Employee_Address, empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.Employee_Address).IsModified = true;
                    }
                    if (olditem.CreatedBy != empCreation.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.CreatedBy, empCreation.CreatedBy, empCreation.otherdata.Remark);
                        scheModel.Entry(empCreation).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), empCreation.otherdata.Remark);
                    scheModel.SaveChanges();

                    string[] grpId = empCreation.otherdata.grpid.Split(',');
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
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], empCreation.Employee_Id, "Employee", " NA", "Create/Update", ex.Message);
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
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
        public ActionResult EmployeeEdit(long id)
        {
            try
            {
                var lstEmp = scheModel.tblEmployees.Where(x => x.Employee_Id == id).FirstOrDefault();
               // if (lstEmp.EmpProfilePhoto != null)
                 //   lstEmp.EmpProfilePhoto = Base64Decode(lstEmp.EmpProfilePhoto);
                return Json(lstEmp, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult EmployeeDeactive(long Id, string remark)
        {
            try
            {
                tblEmployee olditem = oldemployeedata(Id);
                tblEmployee emp = new tblEmployee();
                emp.Employee_Id = Id;
                emp.Status = "Deactive";
                scheModel.tblEmployees.Attach(emp);
                scheModel.Entry(emp).Property(x => x.Status).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Employee_Id, "Employee", olditem.Status.ToString(), emp.Status.ToString(), remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Employee_Id, "Employee", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), remark);
                scheModel.SaveChanges();
                //scheModel.Entry(lstEmp).State = EntityState.Deleted;
                //scheModel.SaveChanges();
                //at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstEmp.Employee_Id, "Employee", "Created Employee", "Delete Data", remark);
                return Json("Data Deactive Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], Id, "Employee", "NA", "Deactive", ex.Message);
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
        public List<tblAccessGroupMaster> lstRole()
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
                return lstGroup;
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
                if (sessionData.SiteId == null)
                {
                    var emp = scheModel.tblEmployees.ToList(); /*Where(y => y.Employee_Department == sessionData.Employee_Department).*/
                    for (int i = 0; i < emp.Count; i++)
                    {
                        lstEmployee.Add(emp[i]);
                    }
                }
                else
                {
                    var emp = scheModel.tblEmployees.Where(y => y.SiteId == sessionData.SiteId).ToList(); /*.*/
                    for (int i = 0; i < emp.Count; i++)
                    {
                        lstEmployee.Add(emp[i]);
                    }

                }
                return lstEmployee;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

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
                    deprtment.Status = "Active";
                    scheModel.tblDepartments.Add(deprtment);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], deprtment.Department_Id, "Department", "NA", "New Department Created", deprtment.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // scheModel.Entry(deprtment).State = EntityState.Modified;
                    tblDepartment oldItem = oldDepartmentdata(deprtment.Department_Id);
                    scheModel.tblDepartments.Attach(deprtment);
                    deprtment.Status = "Active";

                    if (oldItem.Department_Name != deprtment.Department_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.Department_Name, deprtment.Department_Name, deprtment.otherdata.Remark);
                        scheModel.Entry(deprtment).Property(x => x.Department_Name).IsModified = true;
                    }
                    //if (oldItem.Department_Description != deprtment.Department_Description)
                    //{
                    //    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.Department_Description, deprtment.Department_Description, deprtment.otherdata.Remark);
                    //    scheModel.Entry(deprtment).Property(x => x.Department_Description).IsModified = true;
                    //}
                    if (oldItem.Status != deprtment.Status)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.Status.ToString(), deprtment.Status.ToString(), deprtment.otherdata.Remark);
                        scheModel.Entry(deprtment).Property(x => x.CreatedBy).IsModified = true;
                    }

                    if (oldItem.CreatedBy != deprtment.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.CreatedBy.ToString(), deprtment.CreatedBy.ToString(), deprtment.otherdata.Remark);
                        scheModel.Entry(deprtment).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), deprtment.otherdata.Remark);
                    scheModel.SaveChanges();

                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], deprtment.Department_Id, "Department", "NA", "Create/Update", ex.Message);
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
        public ActionResult DepartmentDeactive(long id, string remark)
        {
            try
            {
                tblDepartment oldItem = oldDepartmentdata(id);

                tblDepartment deprtment = new tblDepartment();
                deprtment.Department_Id = id;
                deprtment.Status = "Deactive";
                scheModel.tblDepartments.Attach(deprtment);
                scheModel.Entry(deprtment).Property(x => x.Status).IsModified = true;

                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.Status.ToString(), deprtment.Status.ToString(), remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Department_Id, "Department", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), remark);
                scheModel.SaveChanges();
                //var lstdepartment = scheModel.tblDepartments.Where(x => x.Department_Id == id).FirstOrDefault();
                //scheModel.Entry(lstdepartment).State = EntityState.Deleted;
                //scheModel.SaveChanges();
                //at.InsrtHistory((tblEmployee)Session["EmployeeData"], lstdepartment.Department_Id, "Department", "Created Department", "Delete Department", remark);
                return Json("Data Deactive Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "Department", "NA", "Deactive", ex.Message);
                throw;
            }
        }
        public List<tblDepartment> lstDprt()
        {
            try
            {
                List<tblDepartment> lstdepart = new List<tblDepartment>();
                var SiteId = ((tblEmployee)(Session["EmployeeData"])).SiteId;
                if (SiteId == null)
                {
                    var depart = scheModel.tblDepartments.ToList();
                    for (int i = 0; i < depart.Count; i++)
                    {
                        lstdepart.Add(depart[i]);
                    }
                }
                else
                {
                    var depart = scheModel.tblDepartments.Where(x => x.SiteId == SiteId).ToList();
                    for (int i = 0; i < depart.Count; i++)
                    {
                        lstdepart.Add(depart[i]);
                    }
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
                    designation.Status = "Active";
                    scheModel.tblDesignations.Add(designation);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", "NA", "New Designation Created", designation.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(designation).State = EntityState.Modified;

                    tblDesignation oldItem = oldDesignationdata(designation.Designation_Id);
                    scheModel.tblDesignations.Attach(designation);
                    designation.Status = "Active";
                    if (oldItem.Designation_Name != designation.Designation_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", oldItem.Designation_Name, designation.Designation_Name, designation.otherdata.Remark);
                        scheModel.Entry(designation).Property(x => x.Designation_Name).IsModified = true;

                    }
                    //if (oldItem.Designation_Description != designation.Designation_Description)
                    //{
                    //    at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", oldItem.Designation_Description, designation.Designation_Description, designation.otherdata.Remark);
                    //    scheModel.Entry(designation).Property(x => x.Designation_Description).IsModified = true;
                    //}
                    if (oldItem.CreatedBy != designation.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Designation_Id, "Designation", oldItem.CreatedBy.ToString(), designation.CreatedBy.ToString(), designation.otherdata.Remark);
                        scheModel.Entry(designation).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Designation_Id, "Designation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), designation.otherdata.Remark);
                    scheModel.SaveChanges();
                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], designation.Designation_Id, "Designation", "NA", "Create/Update", ex.Message);
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
        public ActionResult DesignationDeactive(long id, string remark)
        {
            try
            {
                tblDesignation oldItem = oldDesignationdata(id);

                tblDesignation Designation = new tblDesignation();
                Designation.Designation_Id = id;
                Designation.Status = "Deactive";
                scheModel.tblDesignations.Attach(Designation);
                scheModel.Entry(Designation).Property(x => x.Status).IsModified = true;

                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Designation_Id, "Designation", oldItem.Status.ToString(), Designation.Status.ToString(), remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Designation_Id, "Designation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), remark);
                scheModel.SaveChanges();

                return Json("Data Deactive Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "Designation", "NA", "Deactive", ex.Message);
                throw;
            }
        }
        public List<tblDesignation> lstDsgnat()
        {
            try
            {
                List<tblDesignation> lstDsgnat = new List<tblDesignation>();
                var SiteId = ((tblEmployee)(Session["EmployeeData"])).SiteId;
                if (SiteId == null)
                {
                    var desig = scheModel.tblDesignations.ToList();
                    for (int i = 0; i < desig.Count; i++)
                    {
                        lstDsgnat.Add(desig[i]);
                    }
                }
                else
                {
                    var desig = scheModel.tblDesignations.Where(x => x.SiteId == SiteId).ToList();
                    for (int i = 0; i < desig.Count; i++)
                    {
                        lstDsgnat.Add(desig[i]);
                    }
                }

                return lstDsgnat;
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #region
        public ActionResult Workflow()
        {
            return View();
        }

        public ActionResult EmployeeList()
        {
            try
            {
                var emp = scheModel.tblEmployees.Where(x => x.Status == "Active").ToList();
                return Json(new { data = emp }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                return Json(e.Message, JsonRequestBehavior.AllowGet);
                throw;
            }
        }


        public ActionResult RoleList()
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
                    lstGroup.Add(gm);
                }
                return Json(new { data = lstGroup }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
                throw;
            }
        }
        public ActionResult CreateWorkflow(tblWorkFlowMaster wp, List<tblWorkFlowChild> wpC)
        {
            try
            {
                wp.CreatedDate = DateTime.Now;
                wp.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                var isExistm = scheModel.tblWorkFlowMasters.Any(x => x.WorkFlowId == wp.WorkFlowId);
                if (!isExistm)
                {
                    wp.Status = "Active";
                    scheModel.tblWorkFlowMasters.Add(wp);
                    scheModel.SaveChanges();
                    tblWorkFlowChild child;
                    foreach (var item in wpC)
                    {
                        child = new tblWorkFlowChild();
                        child.WorFlowId = wp.WorkFlowId;
                        child.FlowStep = item.FlowStep;
                        child.StepName = item.StepName;
                        child.EmpId = item.EmpId;
                        child.GrpRoleId = item.GrpRoleId;

                        scheModel.tblWorkFlowChilds.Add(child);
                        scheModel.SaveChanges();

                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], wp.WorkFlowId, "WorkFlowMaster", "NA", "New WorkFlow Created", wp.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }


            }
            catch (Exception e)
            {
                throw;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region Equipment
        public ActionResult Equipment()
        {
            ViewBag.lstMachine = lstMachine();
            ViewBag.Groupchild = grpchildList();
            ViewBag.workFlowList = WFList();
            ViewBag.MachineDepartment = fillDepartmentDropDown();
            return View();
        }

        #endregion
        #region Instrument
        public ActionResult Instrument()
        {
            ViewBag.lstMachine = lstMachine();
            ViewBag.Groupchild = grpchildList();
            ViewBag.workFlowList = WFList();
            ViewBag.MachineDepartment = fillDepartmentDropDown();
            return View();
        }
        #endregion
        #region  Equipment/Instrument||Machine
        public string ValidateMachineId(string id)
        {
            try
            {
                var MachineIdStatus = scheModel.tblMachineCreations.Where(m => m.MachineID == id).FirstOrDefault();
                if (MachineIdStatus != null)
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

        public List<tblWorkFlowMaster> WFList()
        {
            try
            {
                List<tblWorkFlowMaster> wflist = new List<tblWorkFlowMaster>();
                var WF = scheModel.tblWorkFlowMasters.Where(x=>x.Status=="Active").ToList();
                for (int i = 0; i < WF.Count; i++)
                {
                    wflist.Add(WF[i]);
                }
                return wflist;
            }
            catch (Exception ex)
            {

                throw;
            }
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
                    if (machine.otherdata.FormName == "Instrument")
                        machine.Instru_Equip_StatusId = 1;
                    else if (machine.otherdata.FormName == "Equipment")
                        machine.Instru_Equip_StatusId = 2;
                    machine.Status = "Active";
                    scheModel.tblMachineCreations.Add(machine);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], machine.Machine_Id, "MachineCreation", "NA", "New Machine Created", machine.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(machine).State = EntityState.Modified;
                    tblMachineCreation olditem = oldMachinedata(machine.Machine_Id);
                    scheModel.tblMachineCreations.Attach(machine);

                    machine.Status = "Active";
                    if (olditem.Machine_Department != machine.Machine_Department)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Department.ToString(), machine.Machine_Department.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Department).IsModified = true;
                    }
                    if (olditem.MachineID != machine.MachineID)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.MachineID.ToString(), machine.MachineID.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Department).IsModified = true;
                    }
                    if (olditem.Machine_Name != machine.Machine_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Name.ToString(), machine.Machine_Name.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Department).IsModified = true;
                    }
                    if (olditem.Machine_Location != machine.Machine_Location)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Location.ToString(), machine.Machine_Location.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Location).IsModified = true;
                    }
                    if (olditem.Machine_Workflow != machine.Machine_Workflow)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Machine_Workflow.ToString(), machine.Machine_Workflow.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.Machine_Location).IsModified = true;
                    }
                    if (olditem.Status != machine.Status)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.Status.ToString(), machine.Status.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.CreatedBy).IsModified = true;
                    }

                    if (olditem.CreatedBy != machine.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.CreatedBy.ToString(), machine.CreatedBy.ToString(), machine.otherdata.Remark);
                        scheModel.Entry(machine).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], olditem.Machine_Id, "MachineCreation", olditem.CreatedDate.ToString(), DateTime.Now.ToString(), machine.otherdata.Remark);

                    scheModel.SaveChanges();

                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], machine.Machine_Id, "MachineCreation", "NA", "Create/Update", ex.Message);
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
        public ActionResult MachineDeActive(long id, string remark)
        {
            try
            {
                tblMachineCreation oldItem = oldMachinedata(id);


                tblMachineCreation machine = new tblMachineCreation();
                machine.Machine_Id = id;
                machine.Status = "Deactive";
                scheModel.tblMachineCreations.Attach(machine);
                scheModel.Entry(machine).Property(x => x.Status).IsModified = true;
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "Designation", oldItem.Status.ToString(), machine.Status.ToString(), remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Machine_Id, "Designation", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), remark);
                scheModel.SaveChanges();

                return Json("Data Deactive Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "MachineCreation", "NA", "Deactive", ex.Message);
                throw;
            }
        }
        public List<tblMachineCreation> lstMachine()
        {
            try
            {
                List<tblMachineCreation> lstMachine = new List<tblMachineCreation>();
                var sessionData = ((tblEmployee)(Session["EmployeeData"]));
                if (sessionData.SiteId == null)
                {
                    var machine = scheModel.tblMachineCreations.Where(y => y.Machine_Department == sessionData.Employee_Department).ToList();
                    for (int i = 0; i < machine.Count; i++)
                    {
                        lstMachine.Add(machine[i]);
                    }
                }
                else
                {
                    var machine = scheModel.tblMachineCreations.Where(y => y.Machine_Department == sessionData.Employee_Department && y.SiteId == sessionData.SiteId).ToList();
                    for (int i = 0; i < machine.Count; i++)
                    {
                        lstMachine.Add(machine[i]);
                    }
                }
                return lstMachine;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        #endregion


        #region Site
        public ActionResult Site()
        {
            ViewBag.lstSite = lstSite();
            ViewBag.Groupchild = grpchildList();
            return View();
        }

        public List<tblSite> lstSite()
        {
            try
            {
                List<tblSite> lstSite = new List<tblSite>();
                var site = scheModel.tblSites.ToList();
                for (int i = 0; i < site.Count; i++)
                {
                    lstSite.Add(site[i]);
                }
                return lstSite;
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        private tblSite oldSitedata(long id)
        {
            dbScheduleModel _context = new dbScheduleModel();
            return _context.tblSites.Single(p => p.Site_Id == id);
        }

        public ActionResult SiteCreation(tblSite site)
        {
            try
            {
                site.CreatedBy = ((tblEmployee)(Session["EmployeeData"])).Employee_Id;
                var isExistm = scheModel.tblSites.Any(x => x.Site_Id == site.Site_Id);
                if (!isExistm)
                {
                    site.CreatedDate = DateTime.Now;
                    site.Status = "Active";
                    scheModel.tblSites.Add(site);
                    scheModel.SaveChanges();
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], site.Site_Id, "Site", "NA", "New Site Created", site.otherdata.Remark);
                    return Json("Save Success", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //scheModel.Entry(designation).State = EntityState.Modified;

                    tblSite oldItem = oldSitedata(site.Site_Id);
                    scheModel.tblSites.Attach(site);
                    site.Status = "Active";
                    if (oldItem.Site_Name != site.Site_Name)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], site.Site_Id, "Site", oldItem.Site_Name, site.Site_Name, site.otherdata.Remark);
                        scheModel.Entry(site).Property(x => x.Site_Name).IsModified = true;
                    }
                    if (oldItem.Site_Description != site.Site_Description)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], site.Site_Id, "Site", oldItem.Site_Description, site.Site_Description, site.otherdata.Remark);
                        scheModel.Entry(site).Property(x => x.Site_Description).IsModified = true;
                    }
                    if (oldItem.CreatedBy != site.CreatedBy)
                    {
                        at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Site_Id, "Site", oldItem.CreatedBy.ToString(), site.CreatedBy.ToString(), site.otherdata.Remark);
                        scheModel.Entry(site).Property(x => x.CreatedBy).IsModified = true;
                    }
                    at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Site_Id, "Site", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), site.otherdata.Remark);
                    scheModel.SaveChanges();
                    return Json("Update Success", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], site.Site_Id, "Site", "NA", "Create/Update", ex.Message);
                throw;
            }
        }

        public ActionResult SiteEdit(long id)
        {

            try
            {
                var lstSite = scheModel.tblSites.Where(x => x.Site_Id == id).FirstOrDefault();
                return Json(lstSite, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        public ActionResult SiteDeactive(long id, string remark)
        {

            try
            {
                tblSite oldItem = oldSitedata(id);

                tblSite site = new tblSite();
                site.Site_Id = id;
                site.Status = "Deactive";
                scheModel.tblSites.Attach(site);
                scheModel.Entry(site).Property(x => x.Status).IsModified = true;

                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Site_Id, "Site", oldItem.Status.ToString(), site.Status.ToString(), remark);
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], oldItem.Site_Id, "Site", oldItem.CreatedDate.ToString(), DateTime.Now.ToString(), remark);
                scheModel.SaveChanges();

                return Json("Data Deactive Successfull", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                at.InsrtHistory((tblEmployee)Session["EmployeeData"], id, "Site", "NA", "Deactive", ex.Message);
                throw;
            }
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