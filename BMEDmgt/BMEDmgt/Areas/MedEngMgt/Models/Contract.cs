using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("Contract")]
    public class Contract
    {
        [Key]
        [Display(Name = "合約案號")]
        [Required]
        public string ContractNo { get; set; }
        [Display(Name = "合約名稱")]
        [Required]
        public string ContractName { get; set; }
        [Display(Name = "合約起始日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}",
            ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Sdate { get; set; }
        [Display(Name = "合約終止日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        [Required]
        public DateTime Edate { get; set; }
        [Display(Name = "合約金額")]
        public int Cost { get; set; }
        [Display(Name = "合約廠商")]
        [Required]
        public int VendorId { get; set; }
        [Display(Name = "廠商名稱")]
        public string VendorName { get; set; }
        [Display(Name = "承辦工程師")]
        [Required]
        public int EngId { get; set; }
        [Display(Name = "工程師姓名")]
        public string EngName { get; set; }
        [Display(Name = "狀態")]
        public string Status { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { set; get; }
        [Display(Name = "異動日期")]
        public DateTime Rtt { get; set; }
    }

}