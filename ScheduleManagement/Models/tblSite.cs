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
    using TrainningManagement.CommonClass;

    public partial class tblSite
    {
        public long Site_Id { get; set; }
        public string Site_Name { get; set; }
        public string Site_Description { get; set; }
        public string Status { get; set; }
        public Nullable<long> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public NewAdd otherdata { get; set; }
    }
}
