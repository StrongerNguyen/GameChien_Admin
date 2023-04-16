using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class CaseRequestModel
    {
        public System.Guid Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Comment { get; set; }
        public string AttachFile { get; set; }
        public int Status { get; set; }
    }
}