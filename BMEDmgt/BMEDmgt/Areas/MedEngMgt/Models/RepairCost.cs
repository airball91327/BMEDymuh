﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
     [Table("RepairCost")]
    public class RepairCost
    {
        [Key, Column(Order = 1)]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "序號")]
        public int SeqNo { get; set; }
        [Display(Name = "零件代號")]
        public string PartNo { get; set; }
        [Required]
        [Display(Name = "零件名稱")]
        public string PartName { get; set; }
        [Display(Name = "規格")]
        public string Standard { get; set; }
        [Required]
        [Display(Name = "單位")]
        public string Unite { get; set; }
        [Required]
        [Display(Name = "單價")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Price { get; set; }
        [Required]
        [Display(Name = "數量")]
        public int Qty { get; set; }
        [Display(Name = "總金額")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal TotalCost { get; set; }
        [Required]
        [Display(Name = "費用別")]
        public string StockType { get; set; }
        [Display(Name = "廠商代號")]
        public int? VendorId { get; set; }
        [Display(Name = "廠商名稱")]
        public string VendorName { get; set; }
        [Display(Name = "日期")]
        public Nullable<DateTime> AccountDate { get; set; }
        [Display(Name = "零用金支付")]
        public string IsPetty { get; set; }
        [Display(Name = "簽單號碼")]
        public string SignNo { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
        public virtual TicketDtl TicketDtl { get; set; }
    }
}