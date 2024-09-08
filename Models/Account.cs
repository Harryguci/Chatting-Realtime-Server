using ChatingApp.Domain.Base;
using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Account: AggregateRoot
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Roles { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime LastLogin { get; set; }
    public virtual ICollection<RoomAccount>? RoomAccounts { get; set; } = new List<RoomAccount>();
}
