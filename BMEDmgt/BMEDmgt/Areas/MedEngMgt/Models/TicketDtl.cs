using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("TicketDtl")]
    public class TicketDtl
    {
        [Key, Column(Order = 1)]
        [Display(Name = "發票號碼")]
        public string TicketDtlNo { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "序號")]
        public int SeqNo { get; set; }
        [Display(Name = "物品名稱")]
        public string ObjName { get; set; }
        [Display(Name = "數量")]
        public int Qty { get; set; }
        [Display(Name = "單位")]
        public string Unite { get; set; }
        [Display(Name = "單價")]
        public decimal Price { get; set; }
        [Display(Name = "金額")]
        public decimal Cost { get; set; }
        [NotMapped]
        [Display(Name = "表單")]
        public string Doctyp { get; set; }
        [NotMapped]
        [Display(Name = "單號")]
        public string Docid { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}