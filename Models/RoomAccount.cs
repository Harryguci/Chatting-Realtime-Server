using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class RoomAccount
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string RoomId { get; set; } = null!;

    public virtual Room? Room { get; set; } = null!;

    public virtual Account? UsernameNavigation { get; set; } = null!;
}
