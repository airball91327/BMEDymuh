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
    [Table("CustOrgan")]
    public class CustOrgan
    {
        [Key]
        [Display(Name = "機構代號")]
        public string CustId { get; set; }
        [Display(Name = "機構名稱")]
        public string CustNam { get; set; }
        [Display(Name = "地址")]
        public string Address { get; set; }
        [Display(Name = "聯絡電話")]
        public string Tel { get; set; }
        [Display(Name = "傳真")]
        public string Fax { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "電子郵件")]
        public string Email { get; set; }
        [Display(Name = "統一編號")]
        public string UniteNo { get; set; }
        [Display(Name = "稅籍地址")]
        public string TaxAddress { get; set; }
        [Display(Name = "稅籍地址區號")]
        public string TaxZipCode { get; set; }
        [Display(Name = "聯絡人姓名")]
        public string Contact { get; set; }
        [Display(Name = "聯絡人電話")]
        public string ContactTel { get; set; }
        [Display(Name = "聯絡人手機")]
        public string ContactMobile { get; set; }
        [DataType(DataType.EmailAddress)]
        [Display(Name = "聯絡人Email")]
        public string ContactEmail { get; set; }
        [Display(Name = "開始日期")]
        public DateTime? StartDate { get; set; }
        [Display(Name = "結束日期")]
        public DateTime? EndDate { get; set; }
        [Display(Name = "狀態")]
        public string Status { get; set; }
        [Display(Name = "類別")]
        public string Kind { get; set; }
        [Display(Name = "群組")]
        public string GroupId { get; set; }

        public static IEnumerable<SelectListItem> GetList()
        {
            BMEDcontext db = new BMEDcontext();
            List<CustOrgan> dt2 = new List<CustOrgan>();
            if (Roles.IsUserInRole(WebSecurity.CurrentUserName,"Admin"))
                dt2 = db.CustOrgans.ToList();
            else if (Roles.IsUserInRole("GroupMgr"))
            {
                //string g = db.CustOrgans.Find(db.AppUsers.Where(p => p.UserName == WebSecurity.CurrentUserName).FirstOrDefault().CustId).GroupId;
                //dt2 = db.CustOrgans.Where(c => c.GroupId == g).ToList();
            }
            else
            {
                AppUser user = db.AppUsers.Where(p => p.UserName == WebSecurity.CurrentUserName).FirstOrDefault();
                if (user != null)
                {
                    //dt2.Add(db.CustOrgans.Find(user.CustId));
                }
            }
                
            return new SelectList(dt2, "CustId", "CustNam", "");
        
        }
    }
}