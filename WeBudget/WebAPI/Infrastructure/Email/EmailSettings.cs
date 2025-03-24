﻿namespace WebAPI.Infrastructure.Email
{


    public class EmailSettings
    {
        public string MailServer { get; set; } = string.Empty;
        public int MailPort { get; set; }
        public string SenderName { get; set; } = string.Empty;
        public string FromEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
