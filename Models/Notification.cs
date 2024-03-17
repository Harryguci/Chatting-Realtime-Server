using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Notification
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public bool? Seen { get; set; }

    public string? Source { get; set; }
}
