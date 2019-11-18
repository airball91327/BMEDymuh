using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;
using System.Web.Mvc;
using WebMatrix.WebData;
using BMEDmgt.Models;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("Department")]
    public class Department
    {
        [Key]
        [Display(Name = "部門代號")]
        public string DptId { get; set; }
        [Display(Name = "中文名稱")]
        public string Name_C { get; set; }
        [Display(Name = "英文名稱")]
        public string Name_E { get; set; }
        [Display(Name = "地理位置")]
        public string Loc { get; set; }
        [Display(Name = "建立日期")]
        public DateTime DateCreated { get; set; }
        [Display(Name = "最後異動日期")]
        public Nullable<DateTime> LastActivityDate { get; set; }

        public static IEnumerable<SelectListItem> GetList()
        {
            BMEDcontext db = new BMEDcontext();
            List<Department> dt = db.Departments.ToList();

            return new SelectList(dt, "DptId", "Name_C", "");
        }
    }
}