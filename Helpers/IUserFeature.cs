namespace ChatingApp.Helpers
{
    public interface IUserFeature
    {
        string Username { get; }
        string Roles { get; }
        string Email { get; }
        DateTime LastLogin { get; }
    }
}
