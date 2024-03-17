namespace ChatingApp.Helpers
{
    public class UserFeature : IUserFeature
    {
        public UserFeature(string username,
            string roles,
            string email,
            DateTime lastLogin)
        {
            Username = username;
            Roles = roles;
            Email = email;
            LastLogin = lastLogin;
        }

        public string Username { get; }
        public string Roles { get; }
        public string Email { get; }
        public DateTime LastLogin { get; }
    }
}
