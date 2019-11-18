using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("Label")]
    public class Label
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "標籤序號")]
        public int LabelId { get; set; }
        [Required]
        [Display(Name = "標籤名稱")]
        public string LabelName { get; set; }
        [Required]
        [Display(Name = "年度")]
        public int Year { get; set; }
        [Required]
        [Display(Name = "開關")]
        public bool Status { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [NotMapped]
        [Display(Name = "異動人員姓名")]
        public string RtpName { get; set; }
        [Display(Name = "異動日期")]
        public DateTime? Rtt { get; set; }
    }

}