using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.ViewModel
{
    public class UserViewModel
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string FullName { get; set; }
        public string RoleName { get; set; }
        public bool isActive { get; set; }
    }
}