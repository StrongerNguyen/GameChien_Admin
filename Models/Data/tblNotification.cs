//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FT_Admin.Models.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class tblNotification
    {
        public System.Guid Id { get; set; }
        public string Type { get; set; }
        public string DeviceName { get; set; }
        public string From { get; set; }
        public string Data { get; set; }
        public string Data2 { get; set; }
        public bool Executed { get; set; }
        public System.DateTime CreatedTime { get; set; }
    }
}
