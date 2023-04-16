using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.RequestModel
{
    public class VipConfigRequestModel
    {
        public System.Guid Id { get; set; }
        public string Name { get; set; }
        public long FromAmount { get; set; }
        public Nullable<long> ToAmount { get; set; }
        public bool isActive { get; set; }
    }
}