using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("AssetPContractPermit")]
    public class AssetPContractPermit
    {
        [Key, Column(Order = 1)]
        [Display(Name = "採購編號")]
        [Required]
        public string PurchaseNo { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "衛署登記證號")]
        [Required]
        public string PermitNo { get; set; }
        [Display(Name = "衛署登記證效期(到期日)")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        public DateTime? PermitValid { get; set; }

        [Display(Name = "狀態")]
        public string Status { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { set; get; }
        [Display(Name = "異動日期")]
        public DateTime Rtt { get; set; }
    }

}