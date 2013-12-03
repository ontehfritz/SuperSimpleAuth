using System;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace SSAManager
{
    public class Email
    {
        private static string _logon = "";
        private const string _password = "";
  
        public static void Send(string domain, string to, string subject,
            string body)
        {

            string from = string.Format ("no-reply@{0}", domain);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_logon, _password)
            };

            using (var message = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body
                })
            {
                ServicePointManager.ServerCertificateValidationCallback = 
                    delegate(object s, X509Certificate certificate, X509Chain chain, 
                        SslPolicyErrors sslPolicyErrors) 
                { return true; };

                smtp.Send(message);
            }
        }
    }
}

