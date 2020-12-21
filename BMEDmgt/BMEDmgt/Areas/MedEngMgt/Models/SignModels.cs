using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    public class UnsignCounts
    {
        public int RepairCount { get; set; }
        public int RepairCount2 { get; set; }
        public int KeepCount { get; set; }
        public int BuyEvalateCount { get; set; }
        public int DeliveryCount { get; set; }
    }
    public class RepairListVModel
    {
        [Display(Name = "類別")]
        public string DocType { get; set; }
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "申請人姓名")]
        public string UserFullName { get; set; }
        [Display(Name = "申請人分機")]
        public string Contact { get; set; }
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "儀器名稱")]
        public string AssetName { get; set; }
        [Display(Name = "申請部門代號")]
        public string ApplyDpt { get; set; }
        [Display(Name = "申請部門")]
        public string ApplyDptName { get; set; }
        [Display(Name = "成本中心代號")]
        public string AccDpt { get; set; }
        [Display(Name = "成本中心名稱")]
        public string AccDptName { get; set; }
        [Display(Name = "維修別")]
        public string RepType { get; set; }
        [Display(Name = "購入日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? BuyDate { get; set; }
        [Display(Name = "購入金額")]
        public decimal? BuyCost { get; set; }
        [Display(Name = "故障描述")]
        public string TroubleDes { get; set; }
        [Display(Name = "處理狀態")]
        public string DealState { get; set; }
        [Display(Name = "處理描述")]
        public string DealDes { get; set; }
        [Display(Name = "費用")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal? Cost { get; set; }
        [Display(Name = "天數")]
        public int? Days { get; set; }
        public string Flg { get; set; }
        public int FlowUid { get; set; }
        [Display(Name = "目前關卡處理人")]
        public string FlowUName { get; set; }
        public string FlowCls { get; set; }
        [Display(Name = "廠商")]
        public string Vendor { get; set; }
        [Display(Name = "設備類別")]
        public string PlantClass { get; set; }
        [Display(Name = "負責工程師")]
        public int? RepEngId { get; set; }
        [Display(Name = "負責工程師")]
        public string RepEngName { get; set; }
        [Display(Name = "數量")]
        public int Amt { get; set; }
        [Display(Name = "送修儀器配件")]
        public string PlantDoc { get; set; }
        [Display(Name = "放置地點")]
        public string PlaceLoc { get; set; }
        [Display(Name = "維修院區")]
        public string RepairArea { get; set; }
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ApplyDate { get; set; }
        [Display(Name = "結案驗收人")]
        public string CheckerName { get; set; }
    }

    public class KeepListVModel
    {
        [Display(Name = "類別")]
        public string DocType { get; set; }
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "申請人姓名")]
        public string UserFullName { get; set; }
        [Display(Name = "申請人分機")]
        public string Contact { get; set; }
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "儀器名稱")]
        public string AssetName { get; set; }
        [Display(Name = "申請部門代號")]
        public string ApplyDpt { get; set; }
        [Display(Name = "申請部門")]
        public string ApplyDptName { get; set; }
        [Display(Name = "成本中心代號")]
        public string AccDpt { get; set; }
        [Display(Name = "成本中心名稱")]
        public string AccDptName { get; set; }
        [Display(Name = "保養方式")]
        public string InOut { get; set; }
        [Display(Name = "保養結果")]
        public string Result { get; set; }
        [Display(Name = "保養描述")]
        public string Memo { get; set; }
        [Display(Name = "費用")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal? Cost { get; set; }
        [Display(Name = "天數")]
        public int? Days { get; set; }
        public string Flg { get; set; }
        public int FlowUid { get; set; }
        [Display(Name = "目前關卡處理人")]
        public string FlowUname { get; set; }
        public string FlowCls { get; set; }
        [Display(Name = "廠商")]
        public string Vendor { get; set; }
        public string Src { get; set; }
        [Display(Name = "保養週期")]
        public int Cycle { get; set; }
        [Display(Name = "保固起始日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? WartySt { get; set; }
        [Display(Name = "保固終止日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? WartyEd { get; set; }
        [Display(Name = "完工日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "負責工程師")]
        public string KeepEngName { get; set; }
    }
}