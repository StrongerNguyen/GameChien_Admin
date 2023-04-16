using FT_Admin.Models;
using FT_Admin.Models.Data;
using FT_Admin.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace FT_Admin.Controllers
{
    [CustomAuthorize("PlayerManage")]
    public class PlayerController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult GetAll(string searchtext, bool isBlock, int? curPage = 1)
        {
            //new RealtimeHub().updateCountNotify();
            //new RealtimeHub().updateCountTicket();
            using (var db = new GameChienEntities())
            {
                string query = @" SELECT *, ROW_NUMBER() over (order by CreatedTime desc) as r
                                  FROM tblPlayer ";
                string where = " WHERE 1 = 1 ";
                if (!string.IsNullOrEmpty(searchtext))
                {
                    where += $" AND ( AccountName like N'%{searchtext}%' or FullName like N'%{searchtext}%' or PhoneNumber like N'%{searchtext}%' or Email like N'%{searchtext}%' or GameAccount like N'%{searchtext}%') ";
                }
                if (isBlock)
                {
                    where += " AND isBlock = 1 ";
                }
                string PageSize = ConfigurationManager.AppSettings["pageSize"].ToString();
                query = "SELECT * FROM ( " + query + where + $") AS kq  where r > ({curPage} - 1) * {PageSize} and r <= {curPage}*{PageSize} order by CreatedTime desc";
                string queryMaxPage = string.Format(@"   Select CASE 
                                                                When COUNT(*)%{0} = 0 
                                                                    then count(*)/{0}
                                                                else ((count(*)/{0}) + 1) 
                                                            end
                                            From tblPlayer
                                            {1}
                                        ", PageSize, where);
                List<PlayerViewModel> dataPlayer = db.Database.SqlQuery<PlayerViewModel>(query).ToList();

                var maxPage = db.Database.SqlQuery<int>(queryMaxPage).FirstOrDefault();
                ViewBag.CurPage = curPage;
                ViewBag.MaxPage = maxPage;
                return PartialView("_PlayerTable", dataPlayer);
            }
        }
        public ActionResult GetById(Guid Id, string searchtext, bool isBlock)
        {
            using (var db = new GameChienEntities())
            {
                var playerFromDB = db.tblPlayers.FirstOrDefault(x => x.Id == Id);
                if (playerFromDB != null
                    && (
                    string.IsNullOrEmpty(searchtext)
                    || (playerFromDB.AccountName?.Contains(searchtext) ?? false)
                    || (playerFromDB.FullName?.Contains(searchtext) ?? false)
                    || (playerFromDB.PhoneNumber?.Contains(searchtext) ?? false)
                    || (playerFromDB.Email?.Contains(searchtext) ?? false)
                    || (playerFromDB.GameAccount?.Contains(searchtext) ?? false)
                    )
                    && (!isBlock || playerFromDB.isBlock)
                    )
                {
                    PlayerViewModel playerView = new PlayerViewModel()
                    {
                        Id = playerFromDB.Id,
                        AccountName = playerFromDB.AccountName,
                        FullName = playerFromDB.FullName,
                        PhoneNumber = playerFromDB.PhoneNumber,
                        GameAccount = playerFromDB.GameAccount,
                        Email = playerFromDB.Email,
                        Avatar = playerFromDB.Avatar,
                        Gender = playerFromDB.Gender,
                        Credit = playerFromDB.Credit,
                        GameLevel = playerFromDB.GameLevel,
                        PercentOfLevelUp = playerFromDB.PercentOfLevelUp,
                        LastIPAddress = playerFromDB.LastIPAddress,
                        LastTimeModified = playerFromDB.LastTimeModified,
                        CreatedTime = playerFromDB.CreatedTime,
                        Status = playerFromDB.Status,
                        isVerifiedGameAccount = playerFromDB.isVerifiedGameAccount,
                        isBlock = playerFromDB.isBlock

                    };
                    return PartialView("_PlayerRow", playerView);
                }
                else
                {
                    return null;
                }
            }
        }
        public ActionResult Detail(Guid Id)
        {
            tblPlayer player;
            using (var db = new GameChienEntities())
            {
                if (Id != null)
                {
                    player = db.tblPlayers.FirstOrDefault(t => t.Id == Id);
                }
                else
                {
                    player = new tblPlayer();
                }
                return PartialView("_Detail", new PlayerViewModel()
                {
                    Id = player.Id,
                    AccountName = player.AccountName,
                    FullName = player.FullName,
                    PhoneNumber = player.PhoneNumber,
                    GameAccount = player.GameAccount,
                    Email = player.Email,
                    Avatar = player.Avatar,
                    Gender = player.Gender,
                    Credit = player.Credit,
                    GameLevel = player.GameLevel,
                    PercentOfLevelUp = player.PercentOfLevelUp,
                    LastIPAddress = player.LastIPAddress,
                    LastTimeModified = player.LastTimeModified,
                    CreatedTime = player.CreatedTime,
                    Status = player.Status,
                    isVerifiedGameAccount = player.isVerifiedGameAccount,
                    isBlock = player.isBlock
                });
            }
        }
        public async Task<JsonResult> Update(PlayerViewModel player)
        {
            //check data
            if (string.IsNullOrEmpty(player.AccountName)) return Json(new { success = false, message = "AccountName không trống." }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(player.FullName)) return Json(new { success = false, message = "FullName không trống." }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(player.PhoneNumber)) return Json(new { success = false, message = "PhoneNumber không trống." }, JsonRequestBehavior.AllowGet);
            if (string.IsNullOrEmpty(player.GameAccount)) return Json(new { success = false, message = "GameAccount không trống." }, JsonRequestBehavior.AllowGet);

            using (var db = new GameChienEntities())
            {
                if (player.Id == Guid.Empty)
                {
                    //thêm mới
                    //check trùng
                    if (db.tblPlayers.Count(t => t.AccountName.Equals(player.AccountName)) > 0) return Json(new { success = false, message = "AccountName đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    if (db.tblPlayers.Count(t => t.PhoneNumber.Equals(player.PhoneNumber)) > 0) return Json(new { success = false, message = "PhoneNumber đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    if (db.tblPlayers.Count(t => t.GameAccount.Equals(player.GameAccount)) > 0) return Json(new { success = false, message = "GameAccount đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    if (!string.IsNullOrEmpty(player.Email) && db.tblPlayers.Count(t => t.Email.Equals(player.Email)) > 0) return Json(new { success = false, message = "Email đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    tblPlayer playerNew = new tblPlayer()
                    {
                        Id = Guid.NewGuid(),
                        AccountName = player.AccountName,
                        Password = "12345678",
                        FullName = player.FullName,
                        PhoneNumber = player.PhoneNumber,
                        GameAccount = player.GameAccount,
                        Email = player.Email,
                        Credit = 0,
                        GameLevel = 0,
                        PercentOfLevelUp = 0,
                        CreatedTime = DateTime.Now,
                        isVerifiedGameAccount = false,
                        isBlock = false
                    };
                    db.tblPlayers.Add(playerNew);
                    db.SaveChanges();
                    return Json(new { success = true, message = "Tạo mới Player thành công.", Id = playerNew.Id }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    //cập nhật
                    tblPlayer playerFromDB = db.tblPlayers.FirstOrDefault(t => t.Id == player.Id);
                    //check trùng
                    if (db.tblPlayers.Count(t => t.AccountName.Equals(player.AccountName) && t.Id != playerFromDB.Id) > 0) return Json(new { success = false, message = "AccountName đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    if (db.tblPlayers.Count(t => t.PhoneNumber.Equals(player.PhoneNumber) && t.Id != playerFromDB.Id) > 0) return Json(new { success = false, message = "PhoneNumber đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    if (db.tblPlayers.Count(t => t.GameAccount.Equals(player.GameAccount) && t.Id != playerFromDB.Id) > 0) return Json(new { success = false, message = "GameAccount đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    if (!string.IsNullOrEmpty(player.Email) && db.tblPlayers.Count(t => t.Email.Equals(player.Email) && t.Id != playerFromDB.Id) > 0) return Json(new { success = false, message = "Email đã tồn tại." }, JsonRequestBehavior.AllowGet);
                    playerFromDB.AccountName = player.AccountName;
                    playerFromDB.FullName = player.FullName;
                    playerFromDB.PhoneNumber = player.PhoneNumber;
                    playerFromDB.GameAccount = player.GameAccount;
                    playerFromDB.Email = player.Email;
                    playerFromDB.isVerifiedGameAccount = player.isVerifiedGameAccount;
                    playerFromDB.isBlock = player.isBlock;
                    db.SaveChanges();
                    return Json(new { success = true, message = "Cập nhật Player thành công.", Id = playerFromDB.Id }, JsonRequestBehavior.AllowGet);
                }
            };
        }
    }
}