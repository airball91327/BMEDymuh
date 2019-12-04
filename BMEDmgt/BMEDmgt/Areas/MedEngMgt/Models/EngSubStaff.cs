using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("EngSubStaff")]
    public class EngSubStaff
    {
        [Key]
        [Display(Name = "工程師代號")]
        public int EngId { get; set; }
        [Required]
        [Display(Name = "代理人代號")]
        public int SubstituteId { get; set; }
        [NotMapped]
        [Display(Name = "代理人")]
        public string SubUserName { get; set; }
        [Required]
        //[DataType(DataType.Date)]
        //[Column(TypeName = "Date")]
        [Display(Name = "開始日期")]
        public DateTime? StartDate { get; set; }
        [Required]
        //[DataType(DataType.Date)]
        //[Column(TypeName = "Date")]
        [Display(Name = "結束日期")]
        public DateTime? EndDate { get; set; }

        [ForeignKey("EngId")]
        public virtual AppUser EngAppUsers { get; set; }
        [ForeignKey("SubstituteId")]
        public virtual AppUser SubAppUsers { get; set; }
    }
}