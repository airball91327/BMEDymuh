﻿using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("Delivery")]
    public class Delivery
    {
        [Key]
        [Display(Name = "表單編號")]
        public string Docid { get; set; }
        [Display(Name = "申請人代號")]
        public int UserId { get; set; }
        [Display(Name = "申請人姓名")]
        public string UserName { get; set; }
        [Display(Name = "所屬單位")]
        public string Company { get; set; }
        [Display(Name = "聯絡方式")]
        public string Contact { get; set; }
        [Display(Name = "申請日期")]
        public DateTime? ApplyDate { get; set; }
        [Display(Name = "成本中心")]
        public string AccDpt { get; set; }
        [NotMapped]
        public string AccDptNam { get; set; }
        [Display(Name = "合約號碼")]
        public string ContractNo { get; set; }
        [Display(Name = "評估案號")]
        public string PurchaseNo { get; set; }
        [Display(Name = "列管編號")]
        public string CrlItemNo { get; set; }
        [Display(Name = "製造號碼")]
        public string MakeNo { get; set; }
        [Display(Name = "交貨人")]
        public string DelivPson { get; set; }
        [NotMapped]
        [Display(Name = "交貨人姓名")]
        public string DelivPsonNam { get; set; }
        [Required]
        [Display(Name = "實際交期")]
        public DateTime DelivDateR { get; set; }
        [Display(Name = "採購人員")]
        public int PurchaserId { get; set; }
        [NotMapped]
        public string PurchaserName { get; set; }
        [Display(Name = "設備工程師")]
        public int EngId { get; set; }
        [NotMapped]
        public string EngName { get; set; }
        [Display(Name = "得標廠商")]
        public string VendorNo { get; set; }
        [Display(Name = "廠商名稱")]
        [NotMapped]
        public string VendorNam { get; set; }
        [NotMapped]
        public string UserDpt { get; set; }
        [Display(Name = "保固金")]
        public Nullable<int> WartyNt { get; set; }
        [Display(Name = "收取日")]
        public DateTime? AcceptDate { get; set; }
        [Display(Name = "保固起始日")]
        public DateTime? WartySt { get; set; }
        [Display(Name = "保固終止日")]
        public DateTime? WartyEd { get; set; }
        [Display(Name = "保固月數")]
        public int WartyMon { get; set; }
        [Display(Name = "檔案驗證日")]
        public DateTime? FileTestDate { get; set; }
        [Display(Name = "驗證者")]
        public Nullable<int> TestUid { get; set; }
        [Display(Name = "規格驗證日")]
        public DateTime? StandardDate { get; set; }
        [Display(Name = "備註說明")]
        public string Memo { get; set; }
        [Display(Name = "報備種類")]
        public string Stype2 { get; set; }
        [Display(Name = "申報設備代碼")]
        public string Code { get; set; }
        [Display(Name = "出廠日期")]
        public DateTime? RelDate { get; set; }
        [Display(Name = "購置日期")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "設備啟用日")]
        public DateTime? OpenDate { get; set; }
        [NotMapped]
        [Display(Name = "預算編號")]
        public string BudgetId { get; set; }

        public string GetID(ref BMEDcontext db)
        {
            string str = "";
            str += "SELECT MAX(DOCID) Docid FROM Delivery ";
            var r = db.Database.SqlQuery(typeof(string), str);
            string did = "";
            int yymm = (System.DateTime.Now.Year - 1911) * 100 + System.DateTime.Now.Month;
            foreach (string s in r)
            {
                did = s;
            }
            if (did != "")
            {
                if (Convert.ToInt64(did) / 100000 == yymm)
                    did = Convert.ToString(Convert.ToInt64(did) + 1);
                else
                    did = Convert.ToString(yymm * 100000 + 1);
            }
            else
            {
                did = Convert.ToString(yymm * 100000 + 1);
            }
            return did;
        }
    }

    [Table("DelivFlow")]
    public class DelivFlow
    {
        [Key, Column(Order = 1)]
        public string Docid { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "關卡號")]
        public int Stepid { get; set; }
        [Display(Name = "關卡人員")]
        public int Userid { get; set; }
        [NotMapped]
        public string UserNam { get; set; }
        [NotMapped]
        [Display(Name = "廠商")]
        public string Vendor { get; set; }
        [NotMapped]
        [Display(Name = "關卡流程提示")]
        public string FlowHint { get; set; }
        [NotMapped]
        public string SelOpin { get; set; }
        [Display(Name = "意見描述")]
        public string Opinions { get; set; }
        [Display(Name = "角色")]
        public string Role { get; set; }
        [Display(Name = "狀態")]
        public string Status { get; set; }
        [Display(Name = "異動人員")]
        public Nullable<int> Rtp { get; set; }
        [Display(Name = "到達時間")]
        public DateTime Rtt { get; set; }
        public Nullable<DateTime> Rdt { get; set; }
        [Display(Name = "關卡")]
        public string Cls { get; set; }
    }

    public class DelivDataVModel
    {
        public string Docid { get; set; }
        [Display(Name = "保固金")]
        public Nullable<int> WartyNt { get; set; }
        [Display(Name = "收取日")]
        public DateTime? AcceptDate { get; set; }
        [Display(Name = "保固起始日")]
        public DateTime? WartySt { get; set; }
        [Display(Name = "保固終止日")]
        public DateTime? WartyEd { get; set; }
        [Display(Name = "工時")]
        public decimal? Hours { get; set; }
        [Display(Name = "費用")]
        public decimal? Cost { get; set; }
        [Display(Name = "保養方式")]
        public string InOut { get; set; }
        [Display(Name = "保養週期")]
        public Nullable<int> Cycle { get; set; }
        [Display(Name = "起始年月")]
        public Nullable<int> KeepYm { get; set; }
        [Display(Name = "檔案驗證日")]
        public DateTime? FileTestDate { get; set; }
        [Display(Name = "驗證者")]
        public Nullable<int> TestUid { get; set; }
        public List<AssetKeep> ak { get; set; }
        [Display(Name = "報備種類")]
        public string Stype2 { get; set; }
        [Display(Name = "申報設備代碼")]
        public string Code { get; set; }
        [Display(Name = "出廠日期")]
        public DateTime? RelDate { get; set; }
        [Display(Name = "購置日期")]
        public DateTime? OrderDate { get; set; }
        [Display(Name = "設備啟用日")]
        public DateTime? OpenDate { get; set; }

    }
}