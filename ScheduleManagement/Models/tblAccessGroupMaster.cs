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
    
    public partial class tblAccessGroupMaster
    {
        public long group_Id { get; set; }
        public string group_Name { get; set; }
        public string remark { get; set; }
        public Nullable<long> SiteId { get; set; }
    }
}