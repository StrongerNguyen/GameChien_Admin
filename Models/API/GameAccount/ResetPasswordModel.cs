﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.API.GameAccount
{
    public class ResetPasswordModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public string loginName { get; set; }
    }
}