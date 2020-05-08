using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("RepairArea")]
    public class RepairArea
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [Display(Name = "維修地點")]
        public string AreaName { get; set; }
    }
}