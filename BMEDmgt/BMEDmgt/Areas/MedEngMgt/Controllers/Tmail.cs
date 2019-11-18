using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace BMEDmgt.Areas.MedEngMgt.Controllers
{
    public class Tmail
    {
        public MailAddress from;
        public MailAddress to;
        public List<MailAddress> cc;
        public string sto;
        public MailMessage message;
        public string server;

        public Tmail()
        {
            message = new MailMessage();
            cc = new List<MailAddress>();
            server = "webmail.ymuh.ym.edu.tw";
            sto = "";
        }

        public string SendMail()
        {
            string msg = "";
            SmtpClient sc = new SmtpClient(server);
            sc.Credentials = new System.Net.NetworkCredential("eao", "Eao12345678");
            message.From = from;
            if (sto != "")
                message.To.Add(sto);
            if (to != null)
                message.To.Add(to);
            foreach (MailAddress d in cc)
            {
                message.Bcc.Add(d);
            }
            try
            {
                sc.Send(message);
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            finally
            {
                sc.Dispose();
            }
            return msg;
        }
    }
}