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
    
    public partial class tblRoomType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public tblRoomType()
        {
            this.tblRooms = new HashSet<tblRoom>();
        }
    
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public int TotalNumOfPlayer { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public bool isActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<tblRoom> tblRooms { get; set; }
    }
}
