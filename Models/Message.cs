using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Message
{
    public string Id { get; set; } = null!;

    public string Username { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string RoomId { get; set; } = null!;

    public string? Filename { get; set; }

    public DateTime? CreateAt { get; set; } = DateTime.Now;

    public DateTime? DeleteAt { get; set; }
}
