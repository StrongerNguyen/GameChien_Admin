using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class RolePermissionViewModel
    {
        public Guid Id { get; set; }
        public string RoleName { get; set; }
        public string PermissionName { get; set; }
    }
}