using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TrainningManagement.Models;

namespace TrainningManagement.CommonClass
{
    public class AuditTrail
    {
        tblHistory history;
        dbScheduleModel dbSche = new dbScheduleModel();

        public void InsrtHistory(tblEmployee emp, long ActionId, string FormName, string oldAction, string newAction, string Remark)
        {
            history = new tblHistory();
            history.Emp_Id = emp.Employee_Id;
            history.EmployeeName = emp.Employee_Name;
            history.ActionRowId = ActionId;
            history.FromName = FormName;
            history.Old_Action = oldAction;
            history.New_Action = newAction;
            history.Remark = Remark;
            history.EntryDatetime = DateTime.Now;
            history.Department_Id = emp.Employee_Department;
            dbSche.tblHistories.Add(history);
            dbSche.SaveChanges();
        }
    }
}