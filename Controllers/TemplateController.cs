using FT_Admin.Models;
using FT_Admin.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("TemplateManage")]
    public class TemplateController : Controller
    {
        // GET: Template
        public ActionResult Index()
        {
            using (var db = new BankAPIEntities())
            {
                return View(db.tblTemplates.ToList());
            }
        }
        public ActionResult Detail(int? id)
        {
            tblTemplate template = null;
            using (var db = new BankAPIEntities())
            {
                if (id != null)
                {
                    template = db.tblTemplates.FirstOrDefault(t => t.Id == id);
                }
            }

            return PartialView("_Detail", template);
        }
        [ValidateInput(false)]
        public async Task<ActionResult> Update(tblTemplate template)
        {
            using (var db = new BankAPIEntities())
            {
                if (template.Id == 0)
                {
                    db.tblTemplates.Add(template);
                    db.SaveChanges();
                    await Logging.LogChangeAsync("AddTemplate", template, User.Identity.Name);
                }
                else
                {
                    tblTemplate templateFromDB = db.tblTemplates.FirstOrDefault(t => t.Id == template.Id);
                    templateFromDB.Name = template.Name;
                    templateFromDB.TemplateContent = template.TemplateContent;
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("UpdateTemplate", template, User.Identity.Name);

                }
                return RedirectToAction("Index", "Template");
            }
        }
    }
}