﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QnyWeb.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ShQnyEntities : DbContext
    {
        public ShQnyEntities()
            : base("name=ShQnyEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<PmsPo> PmsPoes { get; set; }
        public virtual DbSet<PmsPoDetail> PmsPoDetails { get; set; }
        public virtual DbSet<Sku> Skus { get; set; }
        public virtual DbSet<PoiList> PoiLists { get; set; }
        public virtual DbSet<PmsPoView> PmsPoViews { get; set; }
        public virtual DbSet<AspNetUserPois> AspNetUserPois { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<PoiConfig> PoiConfigs { get; set; }
        public virtual DbSet<ReceivingNote> ReceivingNotes { get; set; }
        public virtual DbSet<ReceivingNoteItemView> ReceivingNoteItemViews { get; set; }
        public virtual DbSet<StatisticsByMonthSku> StatisticsByMonthSkus { get; set; }
        public virtual DbSet<StatisticsByDaySku> StatisticsByDaySkus { get; set; }
        public virtual DbSet<StatisticsByDailySkuPoi> StatisticsByDailySkuPois { get; set; }
    }
}