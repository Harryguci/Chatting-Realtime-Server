using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Account
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string Roles { get; set; } = null!;

    public string? Email { get; set; }

    public DateTime? LastLogin { get; set; }

    // public virtual ICollection<FriendRequest>? FriendRequestUser1Navigations { get; set; } = new List<FriendRequest>();

    // public virtual ICollection<FriendRequest>? FriendRequestUser2Navigations { get; set; } = new List<FriendRequest>();

    public virtual ICollection<RoomAccount>? RoomAccounts { get; set; } = new List<RoomAccount>();
}
