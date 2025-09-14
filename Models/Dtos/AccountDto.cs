namespace ChatingApp.Models.Dtos
{
    public class AccountDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;

        public string Password { get; set; } = null!;

        public string Roles { get; set; } = null!;

        public string? Email { get; set; }

        public DateTime? LastLogin { get; set; }
        public virtual List<RoomAccount>? RoomAccounts { get; set; } = new List<RoomAccount>();
    }
}
