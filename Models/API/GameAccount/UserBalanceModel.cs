using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.API.GameAccount
{
    public class UserBalanceModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public double balance { get; set; }
    }
}