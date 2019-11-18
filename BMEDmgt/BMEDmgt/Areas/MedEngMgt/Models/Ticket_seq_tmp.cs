using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using System.Web.Security;

namespace BMEDmgt.Areas.MedEngMgt.Models
{
    [Table("Ticket_seq_tmp")]
    public class Ticket_seq_tmp
    {
        [Key]
        public string YYYMM { get; set; }
        public string TICKET_SEQ { get; set; }
    }
}