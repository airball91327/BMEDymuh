using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("Keep")]
    public class Keep
    {
        [Key]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "申請人代號")]
        public int UserId { get; set; }
        [Display(Name = "申請人姓名")]
        public string UserName { get; set; }
        [Display(Name = "所屬部門")]
        public string DptId { get; set; }
        [Display(Name = "所屬單位")]
        public string Company { get; set; }
        [NotMapped]
        [DataType(DataType.EmailAddress)]
        [Required]
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Display(Name = "聯絡方式")]
        public string Contact { get; set; }
        [Display(Name = "成本中心")]
        public string AccDpt { get; set; }
        [NotMapped]
        public string AccDptName { get; set; }
        [Display(Name = "保養週期")]
        public int Cycle { get; set; }
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "儀器名稱")]
        public string AssetName { get; set; }
        [Display(Name = "放置地點")]
        public string PlaceLoc { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Display(Name = "送單日期")]
        public DateTime? SentDate { get; set; }
        public string Src { get; set; }
        [NotMapped]
        [Display(Name = "購入日")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? BuyDate { get; set; }
    }

    [Table("KeepDtl")]
    public class KeepDtl
    {
        [Key]
        public string DocId { get; set; }
        [Display(Name = "保養結果")]
        public string Result { get; set; }
        [Display(Name = "備註")]
        public string Memo { get; set; }
        [Display(Name = "保養方式")]
        public string InOut { get; set; }
        [Display(Name = "[有][無]費用")]
        public string IsCharged { get; set; }
        [Display(Name = "保養工時")]
        public decimal? Hours { get; set; }
        [Display(Name = "保養費用")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal? Cost { get; set; }
        [Display(Name = "完工日期")]
        public Nullable<DateTime> EndDate { get; set; }
        [Display(Name = "完帳日期")]
        public Nullable<DateTime> CloseDate { get; set; }
        [Display(Name = "關帳日期")]
        public Nullable<DateTime> ShutDate { get; set; }
    }

    [Table("KeepResult")]
    public class KeepResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "保養結果")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public string Flg { get; set; }
    }


    [Table("KeepEmp")]
    public class KeepEmp
    {
        [Key, Column(Order = 1)]
        [Display(Name = "單號")]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "代號")]
        public int UserId { get; set; }
        [NotMapped]
        [Display(Name = "工程師代號")]
        public string UserName { get; set; }
        [NotMapped]
        [Display(Name = "工程師姓名")]
        public string FullName { get; set; }
        [Display(Name = "小時")]
        public int Hour { get; set; }
        [Display(Name = "分鐘")]
        public int Minute { get; set; }

    }

    [Table("KeepFlow")]
    public class KeepFlow
    {
        [Key, Column(Order = 1)]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "關卡號")]
        public int StepId { get; set; }
        [Display(Name = "關卡人員")]
        public int UserId { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        [Display(Name = "廠商")]
        public string Vendor { get; set; }
        [NotMapped]
        [Display(Name = "關卡流程提示")]
        public string FlowHint { get; set; }
        [Display(Name = "意見描述")]
        public string Opinions { get; set; }
        [Display(Name = "角色")]
        public string Role { get; set; }
        [Display(Name = "狀態")]
        public string Status { get; set; }
        [Display(Name = "異動人員")]
        public Nullable<int> Rtp { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
        public Nullable<DateTime> Rdt { get; set; }
        [Display(Name = "關卡")]
        public string Cls { get; set; }
    }

    [Table("KeepCost")]
    public class KeepCost
    {
        [Key, Column(Order = 1)]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "序號")]
        public int SeqNo { get; set; }
        [Display(Name = "零件代號")]
        public string PartNo { get; set; }
        [Display(Name = "零件名稱")]
        public string PartName { get; set; }
        [Display(Name = "規格")]
        public string Standard { get; set; }
        [Display(Name = "單位")]
        public string Unite { get; set; }
        [Display(Name = "單價")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Price { get; set; }
        [Display(Name = "數量")]
        public int Qty { get; set; }
        [Display(Name = "總金額")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal TotalCost { get; set; }
        [Display(Name = "費用別")]
        public string StockType { get; set; }
        [Display(Name = "廠商代號")]
        public int VendorId { get; set; }
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
    [Table("KeepFormat")]
    public class KeepFormat
    {
        [Key]
        [Display(Name = "保養格式代號")]
        public string FormatId { get; set; }
        [Display(Name = "可套用的設備")]
        public string Plants { get; set; }
        [Display(Name = "格式")]
        public string Format { get; set; }
    }
    [Table("KeepFormatDtl")]
    public class KeepFormatDtl
    {
        [Key, Column(Order = 1)]
        [Display(Name = "保養格式代號")]
        public string FormatId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "序號")]
        public int Sno { get; set; }
        [Display(Name = "保養項目描述")]
        public string Descript { get; set; }
        public virtual KeepFormat KeepFormat { get; set; }
    }
    [Table("KeepRecord")]
    public class KeepRecord
    {
        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "格式代號")]
        public string FormatId { get; set; }
        [Key, Column(Order = 3)]
        [Display(Name = "序號")]
        public int Sno { get; set; }
        [Display(Name = "保養項目描述")]
        public string Descript { get; set; }
        [Display(Name = "保養紀錄")]
        public string KeepDes { get; set; }
    }

    public class KeepFormatListVModel
    {
        [Display(Name = "表單編號")]
        public string Docid { get; set; }
        [Display(Name = "格式代號")]
        public string FormatId { get; set; }
        [Display(Name = "儀器名稱")]
        public string Plants { get; set; }
        [Display(Name = "序號")]
        public int Sno { get; set; }
        [Display(Name = "項目描述")]
        public string Descript { get; set; }
        [Display(Name = "保養紀錄")]
        public string KeepDes { get; set; }
    }
}