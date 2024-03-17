using NuGet.Common;

namespace ChatingApp.Models
{
    public class AuthenticateResponse
    {
        public string Id { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string? Email { get; set; }
        public string Roles { get; set; } = null!;
        public string Token { get; set; }
        public AuthenticateResponse(Account user, string token)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            Token = token;
        }
    }
}
