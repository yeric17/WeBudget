using Microsoft.AspNetCore.Identity;

namespace WebAPI.Domain.Users
{
    public class User : IdentityUser
    {
        public string NickName { get; private set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.Now;

        private User()
        {

        }

        public static User Create(string nickName, string email)
        {
            User user = new();
            user.NickName = nickName;
            user.Email = email;
            user.UserName = email;
            user.CreatedAt = DateTimeOffset.UtcNow;
            return user;

        }

    }
}
