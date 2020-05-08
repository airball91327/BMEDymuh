using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("AssetKeep")]
    public class AssetKeep
    {
        [Key]
        [Required]
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [NotMapped]
        [Display(Name = "中文名稱")]
        public string Cname { get; set; }
        [Display(Name = "保養合約")]
        public string ContractNo { get; set; }
        [Display(Name = "保養週期(月)")]
        public Nullable<int> Cycle { get; set; }
        [Display(Name = "起始年月")]
        public Nullable<int> KeepYm { get; set; }
        [Display(Name = "起始年月(手動)")]
        public Nullable<int> KeepYm2 { get; set; }
        [Display(Name = "負責工程師")]
        public int KeepEngId { get; set; }
        [Display(Name = "工程師姓名")]
        public string KeepEngName { get; set; }
        [Display(Name = "保養金額")]
        public Nullable<int> Cost { get; set; }
        [Display(Name = "預定工時")]
        public decimal? Hours { get; set; }
        [Display(Name = "保養方式")]
        public string InOut { get; set; }
        [Display(Name = "WORDPAD保養單貼上")]
        public string FormatId { get; set; }
        [Display(Name = "異動人員")]
        public Nullable<int> Rtp { get; set; }
        [Display(Name = "異動時間")]
        public Nullable<DateTime> Rtt { get; set; }
    }
}