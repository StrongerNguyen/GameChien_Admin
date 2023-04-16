using FT_Admin.Models;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("PromptPlayerManage")]
    public class PromptController : Controller
    {
        // GET: Prompt
        public ActionResult Index()
        {
            using (var db = new BankAPIEntities())
            {
                return View(db.tblPrompts.OrderByDescending(t => t.CreatedTime).ToList());
            }
        }
        public ActionResult Detail(Guid? Id)
        {
            tblPrompt prompt = new tblPrompt();
            using (var db = new BankAPIEntities())
            {
                if (Id != null)
                {
                    prompt = db.tblPrompts.FirstOrDefault(t => t.Id.Equals(Id.Value));
                }
            }
            return PartialView("_Detail", prompt);
        }
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tblPrompt prompt)
        {
            using (var db = new BankAPIEntities())
            {
                if (prompt.Id == Guid.Empty)
                {
                    prompt.Id = Guid.NewGuid();
                    prompt.CreatedTime = DateTime.Now;
                    prompt.CreatedBy = User.Identity.Name;
                    db.tblPrompts.Add(prompt);
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("AddPrompt", prompt, User.Identity.Name);
                }
                else
                {
                    tblPrompt promptFromDB = db.tblPrompts.FirstOrDefault(t => t.Id == prompt.Id);
                    if (promptFromDB == null) RedirectToAction("Index", "Prompt");
                    else
                    {
                        promptFromDB.Content = prompt.Content;
                        promptFromDB.Image = prompt.Image;
                        promptFromDB.PageRoute = prompt.PageRoute;
                        promptFromDB.LastUpdateTime = DateTime.Now;
                        promptFromDB.LastUpdateBy = User.Identity.Name;
                        promptFromDB.isActive = prompt.isActive;
                        await db.SaveChangesAsync();
                        await Logging.LogChangeAsync("UpdatePrompt", prompt, User.Identity.Name);
                    }
                }
                return RedirectToAction("Index", "Prompt");
            }
        }
        public async Task<JsonResult> Delete(Guid Id)
        {
            using (var db = new BankAPIEntities())
            {
                tblPrompt promptFromDB = db.tblPrompts.FirstOrDefault(t => t.Id == Id);
                if (promptFromDB != null)
                {
                    db.tblPrompts.Remove(promptFromDB);
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("UpdatePrompt", promptFromDB, User.Identity.Name);
                }
                return Json(new { success = true });
            }
        }
        public async Task<JsonResult> Upload(HttpPostedFileBase fileUpload, string PageRoute)
        {
            if (fileUpload == null) return Json(new { success = false, message = "Vui lòng tải lên hình ảnh." }, JsonRequestBehavior.AllowGet);
            if (!fileUpload.ContentType.Contains("image/")) return Json(new { success = false, message = "Vui lòng chọn đúng định dạng hình ảnh." }, JsonRequestBehavior.AllowGet);
            if (fileUpload.ContentLength > 10485760) return Json(new { success = false, message = "Kích thước ảnh quá lớn." }, JsonRequestBehavior.AllowGet);
            string domainCustomer = ConfigurationManager.AppSettings["DomainCustomer"].ToString();
            HttpClient client = new HttpClient();
            using (var formData = new MultipartFormDataContent())
            {
                formData.Add(new StreamContent(fileUpload.InputStream), name: "file", fileName: PageRoute + "_" + DateTime.Now.ToString("yyyyMMddHHmm") + "." + fileUpload.FileName.Split('.').Last());
                var response = await client.PostAsync(domainCustomer + "/Upload/Prompt", formData);
                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, attachFile = await response.Content.ReadAsStringAsync() }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }

        }
    }
}