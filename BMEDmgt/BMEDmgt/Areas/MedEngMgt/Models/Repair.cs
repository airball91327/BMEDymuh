using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BMEDmgt.Models;
namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("Repair")]
    public class Repair
    {
        [Key]
        [Required]
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "申請人代號")]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "申請人姓名")]
        public string UserName { get; set; }
        [Required]
        [Display(Name = "所屬部門")]
        public string DptId { get; set; }
        [NotMapped]
        public string DptName { get; set; }
        [NotMapped]
        [DataType(DataType.EmailAddress)]
        [Required]
        [Display(Name = "電子信箱")]
        public string Email { get; set; }
        [Required]
        [Display(Name = "聯絡方式")]
        public string Contact { get; set; }
        [Required]
        [Display(Name = "成本中心")]
        public string AccDpt { get; set; }
        [NotMapped]
        public string AccDptName { get; set; }
        [Required]
        [Display(Name = "維修別")]
        public string RepType { get; set; }
        [Required]
        [Display(Name = "財產編號")]
        public string AssetNo { get; set; }
        [Display(Name = "儀器名稱")]
        public string AssetName { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "請輸入大於0的數字")]
        [Display(Name = "數量")]
        public int Amt { get; set; }
        [Display(Name = "送修儀器配件")]
        public string PlantDoc { get; set; }
        [Required]
        [Display(Name = "放置地點")]
        public string PlaceLoc { get; set; }
        [Display(Name = "申請日期")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? ApplyDate { get; set; }
        [Required]
        [Display(Name = "故障描述")]
        public string TroubleDes { get; set; }
        [Display(Name = "驗收人代號")]
        public int CheckerId { get; set; }
        [Required]
        [Display(Name = "結案驗收人")]
        public string CheckerName { get; set; }
        [Required]
        [Display(Name = "設備類別")]
        public string PlantClass { get; set; }
        [NotMapped]
        [Display(Name = "購入日")]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public DateTime? BuyDate { get; set; }
        [NotMapped]
        [Display(Name = "購入金額")]
        public decimal? Cost { get; set; }
    }

    [Table("NoAssetNoList")]
    public class NoAssetNoList
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "設備名稱")]
        public string Title { get; set; }
        [Required]
        [Display(Name = "狀態")]
        public string Flg { get; set; }
    }

}