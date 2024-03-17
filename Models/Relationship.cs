using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Relationship
{
    public string Id { get; set; } = null!;

    public string User1 { get; set; } = null!;

    public string User2 { get; set; } = null!;

    public string? Type { get; set; }
}
