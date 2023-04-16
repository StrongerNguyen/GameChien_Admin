using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class UserPermissionViewModel
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string PermissionName { get; set; }

    }
}