﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("AssetMaintainContract")]
    public class AssetMaintainContract
    {
        [Key]
        [Display(Name = "契約案號")]
        [Required]
        public string ContractNo { get; set; }
        [Display(Name = "合約名稱")]
        public string ContractName { get; set; }
        [Display(Name = "合約廠商")]
        [Required]
        public int VendorId { get; set; }
        [Required]
        [Display(Name = "廠商名稱")]
        public string VendorName { get; set; }
        [Display(Name = "廠商統一編號")]
        public string VendorUniteNo { get; set; }
        [Required]
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "財產名稱")]
        public string AssetName { get; set; }
        [Display(Name = "廠牌")]
        public string Brand { get; set; }
        [Display(Name = "型號")]
        public string Type { get; set; }
        [Display(Name = "序號")]
        public string SeqNo { get; set; }
        [Display(Name = "台數")]
        public string Qty { get; set; }
        [Display(Name = "單位")]
        public string Unite { get; set; }
        [Display(Name = "合約起始日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Sdate { get; set; }
        [Display(Name = "合約終止日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        public DateTime Edate { get; set; }
        [Display(Name = "保養週期")]
        public int? Cycle { get; set; }
        [Display(Name = "年限")]
        public int? UseLife { get; set; }
        [Display(Name = "總費用")]
        public int TotalCost { get; set; }
        [Display(Name = "費用/年")]
        public int? YearCost { get; set; }
        [Display(Name = "付款分期")]
        public string StagePayment { get; set; }
        [Display(Name = "每期金額")]
        public int? StageCost { get; set; }
        [Display(Name = "合約結束通知")]
        public string EndNotice { get; set; }
        [Display(Name = "備註")]
        public string Note { get; set; }

        [Display(Name = "狀態")]
        public string Status { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { set; get; }
        [Display(Name = "異動日期")]
        public DateTime Rtt { get; set; }
    }

}