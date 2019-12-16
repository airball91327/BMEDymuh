using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("AssetPurchaseContract")]
    public class AssetPurchaseContract
    {
        [Key]
        [Display(Name = "契約案號")]
        [Required]
        public string ContractNo { get; set; }
        [Display(Name = "採購編號")]
        public string PurchaseNo { get; set; }
        [Display(Name = "採購名稱")]
        public string PurchaseName { get; set; }
        [Display(Name = "存置院區")]
        public string LeaveLoc { get; set; }
        [Display(Name = "合約廠商")]
        public int? VendorId { get; set; }
        [Display(Name = "廠商名稱")]
        public string VendorName { get; set; }
        [Display(Name = "廠商統一編號")]
        public string VendorUniteNo { get; set; }
        [Display(Name = "廠商電話")]
        public string VendorPhone { get; set; }
        [Display(Name = "預算金額")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal? Budget { get; set; }
        [Display(Name = "底價金額")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal? BasicPrice { get; set; }
        [Display(Name = "合約類別")]
        public string ContractClass { get; set; }
        [Display(Name = "合約總價")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal? ContractTotalPrice { get; set; }
        [Display(Name = "決標日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? AwardDate { get; set; }
        [Display(Name = "驗收日期")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? AcceptDate { get; set; }
        [Display(Name = "保固期間(年)")]
        public int Warranty { get; set; }
        [Display(Name = "設備類別")]
        public string AssetClass { get; set; }
        [Display(Name = "保固起始日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? WarrantySdate { get; set; }
        [Display(Name = "保固終止日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? WarrantyEdate { get; set; }
        [Display(Name = "保固保證金金額")]
        public int? WarrantyMargin { get; set; }
        [Display(Name = "履約保證金金額")]
        public int? PerformanceMargin { get; set; }
        [Display(Name = "使用單位")]
        public string UseDpt { get; set; }
        [NotMapped]
        public string UseDptName { get; set; }
        [Display(Name = "請購單位")]
        public string PurchaseDpt { get; set; }
        [NotMapped]
        public string PurchaseDptName { get; set; }
        [Display(Name = "採購人員代號")]
        public int? PurchaseUid { get; set; }
        [Display(Name = "採購人員")]
        public string PurchaseUName { get; set; }
        [Display(Name = "醫療器材許可證號")]
        public string PermitNo { get; set; }
        [Display(Name = "許可證效期(到期日)")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? PermitValid { get; set; }
        [Display(Name = "主辦單位")]
        public string Sponsor { get; set; }
        [NotMapped]
        public string SponsorDptName { get; set; }
        [Display(Name = "主辦人員代號")]
        public int? SponsorUid { get; set; }
        [NotMapped]
        [Display(Name = "主辦人員")]
        public string SponsorName { get; set; }
        [Display(Name = "協辦單位")]
        public string CoOrganizer { get; set; }
        [NotMapped]
        public string CoOrganizerDptName { get; set; }
        [Display(Name = "協辦人員代號")]
        public int? CoOrganizerUid { get; set; }
        [NotMapped]
        [Display(Name = "主辦人員")]
        public string CoOrganizerName { get; set; }
        [Display(Name = "採購設備類別")]
        public string PAssetClass { get; set; }
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