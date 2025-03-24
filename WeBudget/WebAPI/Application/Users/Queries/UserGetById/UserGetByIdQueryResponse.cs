namespace WebAPI.Application.Users
{
    public record UserGetByIdQueryResponse(string Id, string? Email, string? UserName);
}
