using System.ComponentModel.DataAnnotations;

namespace WebAPI.Infrastructure.Auth
{
    public class AuthenticationSettings
    {
        public JWT JWT { get; set; } = new JWT();
    }

    public class JWT {

        [Required(ErrorMessage = "JWT Key is required")]
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
