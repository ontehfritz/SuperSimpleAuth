using System;
using System.Net.Mail;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;

namespace SuperSimple.Auth.Api
{
    public class Email
    {
        public string fromAddress = "@gmail.com";
        public string toAddress = "@gmail.com";
        public const string fromPassword = "password";
        public string subject = "";
        public string body = "";


        public Email(){
        }

        public Email(string domain)
        {
            fromAddress = "no-reply@" + domain;
            subject = "Reset password request";

        }

        public void send()
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
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

