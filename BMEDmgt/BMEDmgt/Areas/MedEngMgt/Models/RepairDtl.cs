using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("RepairDtl")]
    public class RepairDtl
    {
        [Key]
        public string DocId { get; set; }
        [Display(Name = "處理狀態")]
        public string DealState { get; set; }
        [Display(Name = "處理描述")]
        public string DealDes { get; set; }
        [Display(Name = "故障原因")]
        public string FailFactor { get; set; }
        [Display(Name = "維修方式")]
        public string InOut { get; set; }
        [Display(Name = "工時")]
        public decimal Hour { get; set; }
        [Display(Name = "[有][無]費用")]
        public string IsCharged { get; set; }
        [Display(Name = "費用")]
        [DisplayFormat(DataFormatString = "{0:0}")]
        public decimal Cost { get; set; }
        [Display(Name = "請購編號")]
        public string PurchaseNo { get; set; }
        [Display(Name = "完工日期")]
        public Nullable<DateTime> EndDate { get; set; }
        [Display(Name = "完帳日期")]
        public Nullable<DateTime> CloseDate { get; set; }
        [Display(Name = "關帳日期")]
        public Nullable<DateTime> ShutDate { get; set; }
        public string CanClose { get; set; }
    }

    [Table("DealStatus")]
    public class DealStatus {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "處理狀況")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public string Flg { get; set; }
    }

    [Table("FailFactor")]
    public class FailFactor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "故障原因")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public string Flg { get; set; }
    }
}