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
    [CustomAuthorize("RecommendManage")]
    public class ContentRecommendController : Controller
    {
        // GET: ContentRecommend
        public ActionResult Index()
        {
            using (var db = new BankAPIEntities())
            {
                return View(db.tblContentRecommends.ToList());
            }
        }
        public ActionResult Detail(int? id)
        {
            tblContentRecommend contentRecommend = null;
            using (var db = new BankAPIEntities())
            {
                if (id != null)
                {
                    contentRecommend = db.tblContentRecommends.FirstOrDefault(t => t.Id == id);
                }
            }

            return PartialView("_Detail", contentRecommend);
        }
        public async Task<ActionResult> Update(tblContentRecommend contentRecommend)
        {
            using (var db = new BankAPIEntities())
            {
                if (contentRecommend.Id == 0)
                {
                    db.tblContentRecommends.Add(contentRecommend);
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("AddContentRecommend", contentRecommend, User.Identity.Name);
                }
                else
                {
                    tblContentRecommend contentRecommendFromDB = db.tblContentRecommends.FirstOrDefault(t => t.Id == contentRecommend.Id);
                    contentRecommendFromDB.Name = contentRecommend.Name;
                    contentRecommendFromDB.Content = contentRecommend.Content;
                    await db.SaveChangesAsync();
                    await Logging.LogChangeAsync("UpdateContentRecommend", contentRecommendFromDB, User.Identity.Name);
                }
                return RedirectToAction("Index", "ContentRecommend");
            }
        }
    }
}