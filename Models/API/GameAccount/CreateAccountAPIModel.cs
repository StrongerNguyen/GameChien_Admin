using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class CreateAccountAPIModel
    {
        public bool status { get; set; }
        public string message { get; set; }
        public List<string> data { get; set; }
    }
}