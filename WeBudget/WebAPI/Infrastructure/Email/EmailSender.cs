using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using WebAPI.Domain.Users;

namespace WebAPI.Infrastructure.Email
{
    public class EmailSender : IEmailSender<User>
    {
        private readonly EmailSettings _emailSettings;
        private readonly EmailTemplateSettings _emailTemplateSettings;
        private string _confirmAccountTemplate;
        private string _resetPasswordTemplate;
        public EmailSender(IOptions<EmailSettings> emailSettings, IOptions<EmailTemplateSettings> emailTemplateSettings)
        {
            _emailSettings = emailSettings.Value;
            _emailTemplateSettings = emailTemplateSettings.Value;

            _confirmAccountTemplate = File.ReadAllText(_emailTemplateSettings.ConfirmAccount.Template);
            _resetPasswordTemplate = File.ReadAllText(_emailTemplateSettings.ResetPassword.Template);
        }

        public Task SendConfirmationLinkAsync(User user, string email, string confirmationLink)
        {

            string body = _confirmAccountTemplate.Replace("{{confirmationLink}}", confirmationLink);

            return SendEmailAsync(email, _emailTemplateSettings.ConfirmAccount.Subject, body);

        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var body = new TextPart(TextFormat.Html);
            body.Text = htmlMessage;

            using var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.FromEmail));
            emailMessage.To.Add(new MailboxAddress(null,email));
            emailMessage.Subject = subject;
            emailMessage.Body = body;

            using var client = new SmtpClient();


            await client.ConnectAsync(_emailSettings.MailServer, _emailSettings.MailPort, true);
            await client.AuthenticateAsync(_emailSettings.FromEmail, _emailSettings.Password);

            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);

        }

        public Task SendPasswordResetCodeAsync(User user, string email, string resetCode)
        {
            return Task.CompletedTask;
        }

        public Task SendPasswordResetLinkAsync(User user, string email, string resetLink)
        {
            string body = _resetPasswordTemplate.Replace("{{resetLink}}", resetLink);

            return SendEmailAsync(email, _emailTemplateSettings.ResetPassword.Subject, body);
        }
    }
}
