using System;
using System.Net;
using System.Net.Mail;

namespace Mothership.Networking
{
    public class EmailSender
    {
        private string smtpServer;
        private int smtpPort;
        private MailAddress smtpUser;
        private NetworkCredential credentials;

        public EmailSender(string smtpServer, int smtpPort, string user, string pass)
        {
            this.smtpServer = smtpServer;
            this.smtpPort = smtpPort;
            smtpUser = new MailAddress(user);
            credentials = new NetworkCredential(smtpUser.Address, pass);
        }

        public void Send(string receiver, string subject, string content)
        {
            var client = new SmtpClient
            {
                Host = smtpServer,
                Port = smtpPort,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = credentials
            };

            using (var message = new MailMessage(smtpUser.Address, receiver, subject, content))
            {
                client.Send(message);
            }
        }
    }
}
