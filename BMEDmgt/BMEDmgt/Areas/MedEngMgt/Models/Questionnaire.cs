using BMEDmgt.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("QuestSend")]
    public class QuestSend
    {
        [Key, Column(Order = 1)]
        [Display(Name = "年月")]
        public string YYYYmm { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "群組代號")]
        public string GroupId { get; set; }
        [Display(Name = "群組名稱")]
        public string GrpName { get; set; }
        [Display(Name = "發送問卷數")]
        public int SendAmt { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { get; set; }
        [NotMapped]
        public string UserName { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }
    }
    [Table("QuestionnaireM")]
    public class QuestionnaireM
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "版本序號")]
        public int VerId { get; set; }
        [Display(Name = "問卷名稱")]
        [Required]
        public string Qname { get; set; }
        [Display(Name = "描述說明")]
        public string Memo { get; set; }
        [Display(Name = "狀態")]
        public string Flg { get; set; }
        [Display(Name = "異動人員")]
        public int Rtp { get; set; }
        [NotMapped]
        public string RtpName { get; set; }
        [Display(Name = "異動日期")]
        public DateTime Rtt { get; set; }

    }
    
    [Table("Questionnaire")]
    public class Questionnaire
    {
        [Key, Column(Order = 1)]
        [Display(Name = "版本序號")]
        public int VerId { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "序號")]
        public int Qid { get; set; }
        [Display(Name = "題目")]
        [Required]
        public string Qtitle { get; set; }
        [Display(Name = "類型")]
        public string Typ { get; set; }
        [Display(Name = "必填")]
        public string Required { get; set; }
    }

    [Table("QuestMain")]
    public class QuestMain
    {
        [Key]
        [Display(Name = "表單編號")]
        public string Docid { get; set; }
        [Display(Name = "年月")]
        public string YYYYmm { get; set; }
        [Display(Name = "機構代號")]
        public string CustId { get; set; }
        [Display(Name = "機構名稱")]
        public string CustNam { get; set; }
        [Display(Name = "合約")]
        public string ContractNo { get; set; }
        [Display(Name = "異動時間")]
        public DateTime Rtt { get; set; }

        public string GetID(ref BMEDcontext db)
        {
            string str = "";
            str += "SELECT MAX(DOCID) RECNO FROM QuestMain ";
            var r = db.Database.SqlQuery(typeof(string), str);
            string did = "";
            int yymm = (System.DateTime.Now.Year - 1911) * 100 + System.DateTime.Now.Month;
            foreach (string s in r)
            {
                did = s;
            }
            if (did != "")
            {
                if (Convert.ToInt64(did) / 100000 == yymm)
                    did = Convert.ToString(Convert.ToInt64(did) + 1);
                else
                    did = Convert.ToString(yymm * 100000 + 1);
            }
            else
            {
                did = Convert.ToString(yymm * 100000 + 1);
            }
            return did;
        }
    }

    [Table("QuestAnswer")]
    public class QuestAnswer
    {
        [Key, Column(Order = 1)]
        [Display(Name = "表單編號")]
        public string Docid { get; set; }
        [Key, Column(Order = 2)]
        [Display(Name = "版本序號")]        
        public int VerId { get; set; }
        [Key, Column(Order = 3)]
        [Display(Name = "序號")]
        public int Qid { get; set; }
        [Display(Name = "答案")]
        public string Answer { get; set; }
    }

    public class QuestReport
    {
        [Display(Name = "時間戳記")]
        public string TimeStamp { set; get; }
        [Display(Name = "合約案號")]
        public string ContractNo { set; get; }
        [Display(Name = "合約名稱")]
        public string Contract { set; get; }
        [Display(Name = "部門代號")]
        public string DptId { get; set; }
        [Display(Name = "部門名稱")]
        public string DptName { get; set; }
        public List<QuestAnswer> Answers { set; get; }

    }

    public class Question
    {
        public int ID { set; get; }
        public int QID { set; get; }
        public string QuestionText { set; get; }
        public List<Answer> Answers { set; get; }
        public string SelectedAnswer { set; get; }
        public string Typ { set; get; }
        public Question()
        {
            Answers = new List<Answer>();
            Answer a;
            for (int i = 1; i <= 5; i++)
            {
                a = new Answer();
                a.ID = i;
                a.AnswerText = Convert.ToString(i);
                Answers.Add(a);
            }
        }
    }

    public class Answer
    {
        public int ID { set; get; }
        public string AnswerText { set; get; }
    }

    public class Evaluation
    {
        public string Qname { get; set; }
        [Required]
        public string Docid { get; set; }
        public string YYYYmm { get; set; }
        [Display(Name = "單位名稱")]
        //[Required]
        public string CustId { get; set; }
        public string CustNam { get; set; }
        [Display(Name = "合約名稱")]
        //[Required]
        public string ContractNo { get; set; }
        public List<Question> Questions { set; get; }
        public Evaluation()
        {
            Questions = new List<Question>();
        }
    }

}