using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Message
{
    public string Id { get; set; } = null!;

    public string? Username { get; set; }

    public string? Friendusername { get; set; }

    public string? Content { get; set; }

    public DateTime? CreateAt { get; set; }
}
