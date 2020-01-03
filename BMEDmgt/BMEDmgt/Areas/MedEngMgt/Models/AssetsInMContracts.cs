using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("AssetsInMContracts")]
    public class AssetsInMContracts
    {
        [Key, Column(Order = 1)]
        [Display(Name = "採購案號")]
        [Required]
        public string PurchaseNo { get; set; }
        [Key, Column(Order = 2)]
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
        public string SerialNo { get; set; }
        [Display(Name = "台數")]
        public string Qty { get; set; }
        [Display(Name = "設備單價")]
        public decimal? UniteCost { get; set; }

        [Display(Name = "異動人員")]
        public int Rtp { set; get; }
        [Display(Name = "異動日期")]
        public DateTime Rtt { get; set; }
    }

}