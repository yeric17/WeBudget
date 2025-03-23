namespace WebAPI.Application.Users
{
    public record UserLoginCommandResponse
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }
}
