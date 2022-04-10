using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Proviser.Services
{
    public class MailServise
    {
        public static async Task SendEmailAsync(string _mail, string _text)
        {
            MailAddress from = new MailAddress("ischenkoyaroslav@gmail.com", "ProviserAndroid");
            MailAddress to = new MailAddress(_mail);
            MailMessage m = new MailMessage(from, to);
            m.Subject = "Судебные заседания " + DateTime.Now.ToShortDateString();
            m.Body = _text;
            SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
            smtp.Credentials = new NetworkCredential("ischenkoyaroslav@gmail.com", "icksxcqinjfcbvpm");
            smtp.EnableSsl = true;
            await smtp.SendMailAsync(m);
            return;
        }
    }
}
