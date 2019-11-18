using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Security;
using System.Web.Mvc;
using WebMatrix.WebData;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("Func")]
    public class Func
    {
        [Key]
        [Display(Name = "功能代號")]
        public string FuncId { get; set; }
        [Required]
        [Display(Name = "模組名稱")]
        public string Module { get; set; }
        [Required]
        [Display(Name = "功能名稱")]
        public string FuncName { get; set; }
        [Display(Name = "狀態")]
        public string Flg { get; set; }
        [Display(Name = "連結路徑")]
        public string Href { get; set; }
       
    }

    public class FuncsInRoles
    {
        [Key, Column(Order = 1)]
        [Display(Name = "功能代號")]
        public string FuncId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "選擇角色")]
        public int RoleId { get; set; }
        [NotMapped]
        [Display(Name = "選擇模組")]
        public string Module { get; set; }
        [NotMapped]
        [Display(Name = "勾選功能權限")]
        public List<RoleInFuncsVModel> InFuncs { get; set; }

    }

    public class RoleInFuncsVModel
    {
        public string FuncId { get; set; }
        public string FuncName { get; set; }
        public bool IsSelected { get; set; }
    }
    }