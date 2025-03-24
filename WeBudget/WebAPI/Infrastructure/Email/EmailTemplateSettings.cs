namespace WebAPI.Infrastructure.Email
{
    public class EmailTemplateSettings
    {
        public EmailTemplate ConfirmAccount { get; set; } = new EmailTemplate();
        public EmailTemplate ResetPassword { get; set; } = new EmailTemplate();
    }

    public class  EmailTemplate
    {
        public string Subject { get; set; } = string.Empty;
        public string Template { get; set; } = string.Empty;

        public string ClientUrl { get; set; } = string.Empty;
    }
}
