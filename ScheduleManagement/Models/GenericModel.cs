using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TrainningManagement.Models
{
    public class GenericModel
    {
        public List<tblDepartment> Department { get; set; }
        public List<tblDesignation> Designation { get; set; }
        public List<tblEmployee>Employee { get; set; }
        public List<tblMachineCreation> Asset { get; set; }
        public List<tblScheduleMaster> Schedule { get; set; }

    }
}