using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public class DataTableResultModel<T>
    {
        public int draw { get; set; }
        public int recordsTotal { get; set; }
        public int recordsFiltered { get; set; }
        public T data { get; set; }
        public string error { get; set; }
    }
}