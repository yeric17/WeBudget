using System.Text.Json.Serialization;

namespace WebAPI.Domain.Users.Entities
{
    public class RefreshToken
    {

        public string RefreshTokenId { get; private set; } = string.Empty;

        [JsonIgnore]
        public User? User { get; }

        public string UserId { get; private set; } = string.Empty;
        public string Token { get; private set; } = string.Empty;
        public DateTimeOffset Expires { get; private set; }
        public bool IsExpired => DateTimeOffset.UtcNow >= Expires;
        public bool IsActive => !IsExpired && Enabled;

        public bool Enabled { get; private set; } = true;

        public static RefreshToken Create( string token,string userId)
        {
            RefreshToken refreshToken = new();
            refreshToken.RefreshTokenId = Guid.NewGuid().ToString();
            refreshToken.UserId = userId;
            refreshToken.Token = token;
            refreshToken.Enabled = true;
            return refreshToken;
        }

        public RefreshToken WithExpires(TimeSpan timeSpan)
        {

            UserId = UserId;
            Token = Token;
            Expires = DateTimeOffset.UtcNow.Add(timeSpan);

            return this;
        }

        public RefreshToken Disable()
        {
            Enabled = false;
            return this;
        }
    }
}
