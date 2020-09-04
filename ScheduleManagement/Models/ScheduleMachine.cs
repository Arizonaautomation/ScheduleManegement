//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TrainningManagement.Models
{
    using System;
    using System.Collections.Generic;

    public partial class ScheduleMachine
    {
        public long Id { get; set; }
        public long MachineId { get; set; }
        public string MachineLocation { get; set; }
        public string ScheduleType { get; set; }
        public Nullable<System.DateTime> StartScheduleDate { get; set; }
        public Nullable<System.DateTime> EndScheduleDate { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public string AsignTo { get; set; }
        public string Status { get; set; }
        public string AsignType { get; set; }
        public Nullable<System.DateTime> ApprovedDate { get; set; }
        public Nullable<long> CreatedDepartment { get; set; }
        public Nullable<long> EmployeeHead { get; set; }
        public string Remark { get; set; }
        public string Button { get; set; }
    }
}
