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
    
    public partial class tblWorkFlowChild
    {
        public long WFChild_Id { get; set; }
        public Nullable<long> WorFlowId { get; set; }
        public string FlowStep { get; set; }
        public string StepName { get; set; }
        public Nullable<long> EmpId { get; set; }
        public Nullable<long> GrpRoleId { get; set; }
    }
}
