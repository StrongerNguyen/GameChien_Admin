﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FT_Admin.Models.Data
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GameChienEntities : DbContext
    {
        public GameChienEntities()
            : base("name=GameChienEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<tblBank> tblBanks { get; set; }
        public virtual DbSet<tblBankAccountByPlayer> tblBankAccountByPlayers { get; set; }
        public virtual DbSet<tblConfigTransactionByMobileApp> tblConfigTransactionByMobileApps { get; set; }
        public virtual DbSet<tblCreditHistory> tblCreditHistories { get; set; }
        public virtual DbSet<tblDeposit> tblDeposits { get; set; }
        public virtual DbSet<tblKickOutRoom> tblKickOutRooms { get; set; }
        public virtual DbSet<tblLogOfMobileApp> tblLogOfMobileApps { get; set; }
        public virtual DbSet<tblNotification> tblNotifications { get; set; }
        public virtual DbSet<tblPermission> tblPermissions { get; set; }
        public virtual DbSet<tblPlayer> tblPlayers { get; set; }
        public virtual DbSet<tblPlayer_Room> tblPlayer_Room { get; set; }
        public virtual DbSet<tblRelationship> tblRelationships { get; set; }
        public virtual DbSet<tblRole> tblRoles { get; set; }
        public virtual DbSet<tblRole_Permission> tblRole_Permission { get; set; }
        public virtual DbSet<tblRoom> tblRooms { get; set; }
        public virtual DbSet<tblRoomFee> tblRoomFees { get; set; }
        public virtual DbSet<tblRoomType> tblRoomTypes { get; set; }
        public virtual DbSet<tblTransaction> tblTransactions { get; set; }
        public virtual DbSet<tblUser> tblUsers { get; set; }
        public virtual DbSet<tblUser_Permission> tblUser_Permission { get; set; }
        public virtual DbSet<tblWithdraw> tblWithdraws { get; set; }
        public virtual DbSet<tblConfig> tblConfigs { get; set; }
    }
}
