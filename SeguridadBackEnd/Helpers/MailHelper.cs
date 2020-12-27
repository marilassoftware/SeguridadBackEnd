using Microsoft.Extensions.Configuration;
using System;
using System.Net.Mail;

namespace SeguridadBackEnd.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendMail(string to, string subject, string body)
        {
            try
            {
                MailMessage m = new MailMessage();
                System.Net.Mail.SmtpClient sc = new System.Net.Mail.SmtpClient();
                try
                {
                    m.From = new MailAddress(_configuration["Mail:From"]);
                    m.To.Add(to);
                    m.Subject = subject;
                    m.IsBodyHtml = true;
                    m.Body = body;
                    sc.Host = _configuration["Mail:Smtp"]; // "smtp.gmail.com";
                    sc.Port = Convert.ToInt32(_configuration["Mail:Port"]);
                    sc.Credentials = new System.Net.NetworkCredential(_configuration["Mail:From"], _configuration["Mail:Password"]);
                    //sc.Credentials = new System.Net.NetworkCredential("marilassoftware@gmail.com", "SomosLosMejores*100");

                    sc.EnableSsl = true;
                    sc.Send(m);
                }
                catch (Exception ex)
                {

                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
