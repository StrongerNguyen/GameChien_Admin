using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models.Dto
{
    public class CaseDto
    {
        public System.Guid Id { get; set; }
        public string Title { get; set; }
        public string Detail { get; set; }
        public string Comment { get; set; }
        public string AttachFile { get; set; }
        public string CreatedBy { get; set; }
        public System.DateTime CreatedTime { get; set; }
        public string LastUpdateBy { get; set; }
        public Nullable<System.DateTime> LastUpdateTime { get; set; }
        public int Status { get; set; }
    }
}