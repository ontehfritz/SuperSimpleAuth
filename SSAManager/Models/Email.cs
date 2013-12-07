using System;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Configuration;

namespace SSAManager
{
    public class Email
    {
        private static string _logon =      ConfigurationManager.AppSettings.Get("smtp:Logon");
        private static string _password =   ConfigurationManager.AppSettings.Get("smtp:Password");
        private static string _smtp =       ConfigurationManager.AppSettings.Get("smtp:Server");
        private static string _port =       ConfigurationManager.AppSettings.Get("smtp:Port");
  
        public static void Send(string domain, string to, string subject,
            string body)
        {

            string from = string.Format ("no-reply@{0}", domain);

            var smtp = new SmtpClient(_smtp, int.Parse(_port))
            {
                Credentials = new NetworkCredential(_logon, _password),
                EnableSsl = true
                //DeliveryMethod = SmtpDeliveryMethod.Network,
                //UseDefaultCredentials = false,
            };

            using (var message = new MailMessage(from, to){
                    Subject = subject,
                    Body = body})
            {
                ServicePointManager.ServerCertificateValidationCallback = 
                    delegate(object s, X509Certificate certificate, X509Chain chain, 
                        SslPolicyErrors sslPolicyErrors) 
                { 
                    return true; 
                };

                smtp.Send(message);
            }
        }
    }
}

