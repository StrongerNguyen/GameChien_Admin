using FT_Admin.Models.Data;
using FT_Admin.Models.Enum;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FT_Admin.Models
{
    public static class Logging
    {
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static async Task LogToDBAsync(string source, object exception, object data = null)
        {
            try
            {
                //using (var db = new BankAPIEntities())
                //{
                //    db.tblLogs.Add(new tblLog()
                //    {
                //        Source = source,
                //        Exception = exception?.ToString() ?? "",
                //        Data = data?.ToString() ?? "",
                //        CreatedDate = DateTime.Now
                //    });
                //    await db.SaveChangesAsync();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static void LogToDB(string source, object exception, object data = null)
        {
            try
            {
                //using (var db = new BankAPIEntities())
                //{
                //    db.tblLogs.Add(new tblLog()
                //    {
                //        Source = source,
                //        Exception = exception?.ToString() ?? "",
                //        Data = data?.ToString() ?? "",
                //        CreatedDate = DateTime.Now
                //    });
                //    db.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static async Task LogChangeAsync(string Name, object Data, string ChangeBy)
        {
            try
            {
                //using (var db = new BankAPIEntities())
                //{
                //    db.tblLogChanges.Add(new tblLogChange()
                //    {
                //        Name = Name,
                //        Data = JsonConvert.SerializeObject(Data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                //        CreatedBy = ChangeBy,
                //        CreatedTime = DateTime.Now
                //    });
                //    await db.SaveChangesAsync();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }
        public static Task LogChange(string Name, object Data, string ChangeBy)
        {
            try
            {
                //using (var db = new BankAPIEntities())
                //{
                //    db.tblLogChanges.Add(new tblLogChange()
                //    {
                //        Name = Name,
                //        Data = JsonConvert.SerializeObject(Data, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore }),
                //        CreatedBy = ChangeBy,
                //        CreatedTime = DateTime.Now
                //    });
                //    db.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return Task.CompletedTask;
        }
    }
}