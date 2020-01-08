﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("AssetFile")]
    public class AssetFile
    {
        [Key, Column(Order = 1)]
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "序號")]
        public int SeqNo { get; set; }
        [Key, Column(Order = 3)]
        [Display(Name = "檔案序號")]
        public int Fid { get; set; }
        [Required]
        [Display(Name = "摘要")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "檔案連結")]
        public string FileLink { get; set; }
        [Display(Name = "異動人員")]
        public string Rtp { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
    }

    public class CopyToFile
    {
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "序號")]
        public int SeqNo { get; set; }
        
    }
}