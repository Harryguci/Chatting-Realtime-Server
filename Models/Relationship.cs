using System;
using System.Collections.Generic;

namespace ChatingApp.Models;

public partial class Relationship
{
    public string User1 { get; set; } = null!;

    public string User2 { get; set; } = null!;

    public string LevelFriend { get; set; } = null!;

    public virtual Account User1Navigation { get; set; } = null!;

    public virtual Account User2Navigation { get; set; } = null!;
}
