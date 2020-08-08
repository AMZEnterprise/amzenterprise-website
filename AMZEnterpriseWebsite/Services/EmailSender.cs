using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace AMZEnterpriseWebsite.Services
{
    // This class is used by the application to send email for account confirmation and password reset.
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            string emailSenderUser = _configuration.GetSection("EmailService").GetSection("Username").Value;
            string emailSenderPassword = _configuration.GetSection("EmailService").GetSection("Password").Value;

            string smtpHost = _configuration.GetSection("EmailService").GetSection("SMTPHost").Value;
            int smtpPort = int.Parse(_configuration.GetSection("EmailService").GetSection("SMTPPort").Value);

            SmtpClient client = new SmtpClient(smtpHost,smtpPort)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(emailSenderUser, emailSenderPassword),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(emailSenderUser)
            };

            mailMessage.To.Add(email);
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;
            mailMessage.Subject = subject;
            client.Send(mailMessage);

            return Task.CompletedTask;
        }
    }
}
