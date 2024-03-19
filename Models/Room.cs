using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Room
{
    public string Id { get; set; } = null!;

    public string Type { get; set; } = null!;

    public virtual ICollection<RoomAccount>? RoomAccounts { get; set; } = new List<RoomAccount>();
}
