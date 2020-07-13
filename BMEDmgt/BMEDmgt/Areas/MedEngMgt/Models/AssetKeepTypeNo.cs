using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{

    [Table("AssetKeepTypeNo")]
    public class AssetKeepTypeNo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "保養週期")]
        public int Id { get; set; }
        [Required]
        [Display(Name = "保養週期描述")]
        public string Descirption { get; set; }
    }
}