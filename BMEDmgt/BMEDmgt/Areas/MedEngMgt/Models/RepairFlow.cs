using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("RepairFlow")]
    public class RepairFlow
    {
         [Key, Column(Order=1)]
         public string DocId { get; set; }
         [Key, Column(Order=2)]
         [Display(Name = "關卡號")]
         public int StepId { get; set; }
         [Display(Name = "關卡人員")]
         public int UserId { get; set; }
         [NotMapped]
         public string UserName { get; set; }
         [NotMapped]
         [Display(Name = "廠商")]
         public string Vendor { get; set; }
         [NotMapped]
         [Display(Name = "關卡流程提示")]
         public string FlowHint { get; set; }
         [Display(Name = "意見描述")]
         public string Opinions { get; set; }
         [Display(Name = "角色")]
         public string Role { get; set; }
         [Display(Name = "狀態")]
         public string Status { get; set; }
         [Display(Name = "異動人員")]
         public Nullable<int> Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動時間")]
         public DateTime Rtt { get; set; }
         public Nullable<DateTime> Rdt { get; set; }
         [Display(Name = "關卡")]
         public string Cls { get; set; }
    }

    public class Assign
    {
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "流程提示")]
        public string Hint { get; set; }
        [Required]
        [Display(Name = "簽核選項")]
        public string AssignCls { get; set; }
        [Display(Name = "意見描述")]
        public string AssignOpn { get; set; }
        [Required]
        [Display(Name = "流程關卡")]
        public string FlowCls { get; set; }
        [Display(Name = "廠商")]
        public string FlowVendor { get; set; }
        public string VendorName { get; set; }
        [Required]
        [Display(Name = "關卡人員")]
        public int? FlowUid { get; set; }
        public string ClsNow { get; set; }
        [Display(Name = "允許驗收人結案?")]
        public bool CanClose { get; set; }
        public string AssetNo { get; set; }
    }
}