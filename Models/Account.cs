using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? Roles { get; set; }

    public DateTime? Datebirth { get; set; }

    public string? Emaill { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? DeleteAt { get; set; }
}
