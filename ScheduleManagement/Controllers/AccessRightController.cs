using System;
using System.Collections.Generic;
using System.Data.Entity;
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

    public class AccessRightController : Controller
    {
        dbScheduleModel scheModel = new dbScheduleModel();

        // GET: AccessRight
        public ActionResult Index()
        {
            ViewBag.Groupchild = grpchildList();
            ViewBag.Menu = menuList();
            ViewBag.grpList = bindData();
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

        public List<tblAccessMenuMaster> menuList()
        {
            try
            {
                return scheModel.tblAccessMenuMasters.ToList();
                // return Json(scheModel.tblAccessMenuMasters.ToList(), JsonRequestBehavior.AllowGet);
            }

            catch (Exception e) { throw; }

        }
        public List<tblAccessGroupMaster> bindData()
        {
            //var AccessGroupChildDetail = scheModel.tblAccessGroupChilds
            //.Join(
            //      scheModel.tblAccessGroupMasters,
            //      GC => GC.group_Id,
            //      GM => GM.group_Id,
            //      (GC, GM) => new { GC, GM })
            //.Join(
            //      scheModel.tblAccessMenuMasters,
            //      GCC => GCC.GC.menu_Id,
            //      MM => MM.menu_Id,
            //      (GCC, MM) => new
            //      {
            //          GroupID = GCC.GC.group_Id,
            //          GroupName = GCC.GM.group_Name,
            //          GroupDescription = GCC.GM.remark,
            //          MenuID = GCC.GC.menu_Id,
            //          MenuName = MM.menu_Name,
            //      }).ToList();
            try
            {
                List<tblAccessGroupMaster> Grouplist = new List<tblAccessGroupMaster>();
                var SessionData = ((tblEmployee)(Session["EmployeeData"]));
                if (SessionData.SiteId == null)
                    Grouplist = scheModel.tblAccessGroupMasters.ToList();
                else
                    Grouplist = scheModel.tblAccessGroupMasters.Where(x => x.SiteId == SessionData.SiteId).ToList();
                return Grouplist;
            }

            catch (Exception e) { throw; }

        }
        public string ValidateGroup(string GroupName)
        {
            try
            {
                var GroupStatus = scheModel.tblAccessGroupMasters.Where(m => m.group_Name == GroupName).FirstOrDefault();
                if (GroupStatus != null)
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
        public ActionResult CreateAccessGroup(tblAccessGroupMaster gm)
        {
            try
            {
                var msg = "";
                var isExist = scheModel.tblAccessGroupMasters.Any(x => x.group_Id == gm.group_Id);
                if (!isExist)
                {
                    scheModel.tblAccessGroupMasters.Add(gm);
                    scheModel.SaveChanges();
                    List<string> list = new List<string>();
                    var id = gm.group_Id;
                    msg = "Add";
                    return Json(new { id, msg }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    scheModel.Entry(gm).State = EntityState.Modified;
                    scheModel.SaveChanges();
                    msg = "Update";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }

            catch (Exception e) { throw; }
        }

        public ActionResult AddAccessGroupChild(List<tblAccessGroupChild> lst)
        {

            try
            {
                var msg = "";
                tblAccessGroupChild groupId = new tblAccessGroupChild();
                tblAccessGroupChild grpchild = new tblAccessGroupChild();
                foreach (var item in lst)
                {

                    if (item.group_Id != null)
                    {
                        groupId.group_Id = item.group_Id;
                        break;
                    }
                }
                scheModel.tblAccessGroupChilds.RemoveRange(scheModel.tblAccessGroupChilds.Where(c => c.group_Id == groupId.group_Id));
                scheModel.SaveChanges();

                foreach (var itemlist in lst)
                {
                    if (itemlist.group_Id == null)
                    {
                        grpchild.group_Id = groupId.group_Id;
                        grpchild.menu_Id = itemlist.menu_Id;
                        grpchild.access_Create = itemlist.access_Create;
                        grpchild.access_Modify = itemlist.access_Modify;
                        grpchild.access_Delete = itemlist.access_Delete;
                        grpchild.access_View = itemlist.access_View;

                        scheModel.tblAccessGroupChilds.Add(grpchild);
                        scheModel.SaveChanges();
                    }
                }
                msg = "Add";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }

            catch (Exception e) { throw; }

        }


        public ActionResult editGroupName(long id)
        {
            //var AccessGroupChildDetail = scheModel.tblAccessGroupMasters.Where(x => x.group_Id == groupId)
            //.Join(
            //      scheModel.tblAccessGroupChilds,
            //      GM =>GM.group_Id,
            //      GC => GC.group_Id,
            //      (GM, GC) => new
            //      {
            //          GroupID = GM.group_Id,
            //          GroupName =GM.group_Name,
            //          GroupDescription =GM.remark,
            //          MenuID = GC.menu_Id,
            //          AccessCreate = GC.access_Create,
            //          AccessModify=GC.access_Modify,
            //          AccessDelete=GC.access_Delete,
            //      }).ToList();
            try
            {
                var GroupDetail = scheModel.tblAccessGroupMasters.Where(m => m.group_Id == id).FirstOrDefault();
                return Json(GroupDetail, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }

        }
        public ActionResult editGroupChild(long id)
        {
            try
            {
                var AccessGroupChildDetail = scheModel.tblAccessGroupChilds.Where(x => (x.access_Create == "true" || x.access_Modify == "true" || x.access_Delete == "true" || x.access_View == "true") && x.group_Id == id).ToList();
                return Json(new { data = AccessGroupChildDetail }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }


        }

        public ActionResult EmployeeCount(long Id)
        {
            try
            {
                var EmplyeeCount = scheModel.tblAccessGroupMasters.Where(x => x.group_Id == Id)
           .Join(
                 scheModel.tblEmployees,
                 GM => GM.group_Id,
                 E => E.group_Id,
                 (GM, E) => new
                 {
                     employeeId = E.Employee_Id
                 }).Count();

                return Json(EmplyeeCount, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }


        }
        public ActionResult DeleteGroup(long Id)
        {
            try
            {
                var deleteLine = scheModel.tblAccessGroupMasters.Where(x => x.group_Id == Id).FirstOrDefault();
                scheModel.Entry(deleteLine).State = EntityState.Deleted;
                scheModel.SaveChanges();
                return Json("Deleted Successfuly", JsonRequestBehavior.AllowGet);
            }
            catch (Exception e) { throw; }


        }
    }
}