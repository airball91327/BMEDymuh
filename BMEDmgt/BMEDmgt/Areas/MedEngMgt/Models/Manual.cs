using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("Manual")]
    public class Manual
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int FileId { get; set; }
        [Display(Name = "手冊名稱")]
        public string ManualName { get; set; }
        [Required]
        [Display(Name = "設備型號*")]
        public string AssetType { get; set; }
        [Required]
        [Display(Name = "設備廠牌*")]
        public string AssetBrand { get; set; }
        [Required]
        [Display(Name = "手冊類別*")]
        public string ManualClass { get; set; }
        [Required]
        [Display(Name = "檔案類型*")]
        public string FileType { get; set; }
        [NotMapped]
        [Display(Name = "新增檔案類型")]
        public string NewFileType { get; set; }
        [Display(Name = "使用語言")]
        public string FileLanguage { get; set; }
        [Required]
        [Display(Name = "檔案名稱*")]
        public string FileName { get; set; }
        [Display(Name = "檔案UID")]
        public string FileGuid { get; set; }
        [Display(Name = "檔案路徑")]
        public string FilePath { get; set; }
        [Display(Name = "查閱權限*")]
        public int FileAuth { get; set; }
        [Display(Name = "備註說明")]
        public string Remark { get; set; }
        [Display(Name = "異動人員")]
        public int? Rtp { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
        [NotMapped]
        [Display(Name = "手冊查詢來源")]
        public string SearchFrom { get; set; }
    }
}