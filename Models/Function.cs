using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace FT_Admin.Models
{
    public static class Function
    {
        public static string UploadImage(string domain, HttpPostedFileBase file, string fileName)
        {
            fileName = fileName + "." + file.FileName.Split('.').Last();
            string folder = HttpContext.Current.Server.MapPath("/Upload/Images/");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            file.SaveAs(folder + fileName);
            return domain + "/Upload/Images/" + fileName;
        }
    }
}