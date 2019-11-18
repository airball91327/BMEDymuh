using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("DeptStok")]
    public class DeptStok
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "材料編號")]
        public int StokId { get; set; }
        [Required]
        [Display(Name = "庫存類別")]
        public string StokCls { get; set; }
        [Required]
        [Display(Name = "材料編號")]
        public string StokNo { get; set; }
        [Required]
        [Display(Name = "材料名稱")]
        public string StokNam { get; set; }
        [Display(Name = "單位")]
        public string Unite { get; set; }
        [Display(Name = "單價")]
        [DisplayFormat(DataFormatString = "{0:0}", ApplyFormatInEditMode = true)]
        public decimal Price { get; set; }
        [Display(Name = "數量")]
        public int Qty { get; set; }
        [Display(Name = "安全存量")]
        public int SafeQty { get; set; }
        [Display(Name = "庫存地點")]
        public string Loc { get; set; }
        [Display(Name = "規格")]
        public string Standard { get; set; }
        [Display(Name = "零件廠牌")]
        public string Brand { get; set; }
        [Display(Name = "狀態")]
        [Required]
        public string Status { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
        [Display(Name = "機構")]
        public string CustOrgan_CustId { get; set; }

        //public static IEnumerable<SelectListItem> GetBrandList()
        //{
        //    MvcMedEngMgrContext db = new MvcMedEngMgrContext();
        //    IEnumerable<string> sv = db.Database.SqlQuery<string>("SELECT DISTINCT BRAND FROM DEPTSTOK");
        //    List<SelectListItem> listItem = new List<SelectListItem>();
        //    foreach (string s in sv)
        //    {
        //        listItem.Add(new SelectListItem { Text = s, Value = s });
        //    }
        //    return listItem;
        //}
    }

    [Table("StokRecord")]
    public class StokRecord
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "進出貨紀錄編號")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "物料編號")]
        public string StokNo { get; set; }
        [Required]
        [Display(Name = "物料名稱")]
        public string StokNam { get; set; }
        [Required]
        [Display(Name = "進/出庫")]
        public string InOut { get; set; }
        [Required]
        [Display(Name = "庫存量")]
        public int TotalAmt { get; set; }
        [Required]
        [Display(Name = "數量")]
        public int Amt { get; set; }
        [Required]
        [Display(Name = "類別")]
        public string RecordType { get; set; }
        [Display(Name = "表單編號")]
        public string DocId { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
    }
}