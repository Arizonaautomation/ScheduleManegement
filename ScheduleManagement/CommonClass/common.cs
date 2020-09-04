using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;

namespace TrainningManagement.CommonClass
{
    public class common
    {
        #region 
        public string fromEmailAddress { get; set; }
        public string toEmailAddress { get; set; }
        public string messageBody { get; set; }
        public string htmlString { get; set; }
        public string password { get; set; }
        public string mailSubject { get; set; }
        #endregion
        public string sendMail(common cm)
        {
            try
            {
                MailMessage message = new MailMessage();
                SmtpClient smtp = new SmtpClient();
                message.From = new MailAddress(cm.fromEmailAddress);//Enter From Mail Id
                message.To.Add(new MailAddress(cm.toEmailAddress));//Enter To Mail Id
                message.Subject = mailSubject;//Enter Subject Name
                message.IsBodyHtml = true;//Enter Message Html
                message.Body = messageBody;
                smtp.Port = 587;
                smtp.Host = "smtp.gmail.com"; //for gmail host
                smtp.EnableSsl = true;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(cm.fromEmailAddress, cm.password);//Enter mail address and Password
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(message);
                return "Mail Send Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}