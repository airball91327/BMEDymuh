﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("Ticket")]
    public class Ticket
    {
        [Key]
        [Display(Name = "發票號碼")]
        public string TicketNo { get; set; }
        [Display(Name = "發票日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? TicDate { get; set; }
        [Display(Name = "廠商代號")]
        public int? VendorId { get; set; }
        [Display(Name = "廠商名稱")]
        public string VendorName { get; set; }
        [Display(Name = "總價")]
        public int TotalAmt { get; set; }
        [Display(Name = "稅額")]
        public int TaxAmt { get; set; }
        [NotMapped]
        [Display(Name = "累計")]
        public decimal Amt { get; set; }
        [Display(Name = "備註")]
        public string Note { get; set; }
        [Display(Name = "殘值")]
        public int ScrapValue { get; set; }
        [Display(Name = "請款日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ApplyDate { get; set; }
        [Display(Name = "取消日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? CancelDate { get; set; }
        public ICollection<TicketDtl> TicketDtls { get; set; }
    }

}