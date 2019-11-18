using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;
using WebMatrix.WebData;
using System.Data.SqlClient;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    public class RepairPrintVModel
    {
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
        [Display(Name = "成本中心")]
        public string AccDpt { get; set; }
        public string AccDptNam { get; set; }
        [Display(Name = "維修別")]
        public string RepType { get; set; }
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "儀器名稱")]
        public string AssetNam { get; set; }
        [Display(Name = "數量")]
        public int Amt { get; set; }
        [Display(Name = "送修儀器附件")]
        public string PlantDoc { get; set; }
        [Display(Name = "放置地點")]
        public string PlaceLoc { get; set; }
        [Display(Name = "申請日期")]
        public DateTime? ApplyDate { get; set; }
        [Display(Name = "故障描述")]
        public string TroubleDes { get; set; }
        [Display(Name = "完工日期")]
        public Nullable<DateTime> EndDate { get; set; }
        [Display(Name = "驗收日期")]
        public Nullable<DateTime> CloseDate { get; set; }
        [Display(Name = "處理描述")]
        public string DealDes { get; set; }
        [Display(Name = "工時")]
        public decimal Hour { get; set; }
        [Display(Name = "維修方式")]
        public string InOut { get; set; }
        [Display(Name = "工程師")]
        public string EngName { get; set; }
        [Display(Name = "驗收人代號")]
        public string DelivEmp { get; set; }
        [Display(Name = "驗收人姓名")]
        public string DelivEmpName { get; set; }
        [Display(Name = "購入日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? BuyDate { get; set; }
        [Display(Name = "購入金額")]
        [DisplayFormat(DataFormatString = "{0:N0}")]
        public decimal? Cost { get; set; }
    }
}