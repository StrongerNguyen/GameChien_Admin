using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.API.GameAccount
{
    public class CheckLockedModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public int locked { get; set; }
    }
}