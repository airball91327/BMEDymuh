using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("RepairDtlRecord")]
    public class RepairDtlRecord
    {
        [Key, Column(Order = 1)]
        public string DocId { get; set; }
        [Key, Column(Order = 2)]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "序列號")]
        public int SeqNo { get; set; }
        [Display(Name = "代號")]
        public int UserId { get; set; }
        [NotMapped]
        [Display(Name = "工程師代號")]
        public string UserName { get; set; }
        [NotMapped]
        [Display(Name = "工程師姓名")]
        public string FullName { get; set; }
        [Display(Name = "處理描述")]
        public string DealDes { get; set; }
        [Display(Name = "紀錄時間")]
        public DateTime? Rtt { get; set; }
    }
}