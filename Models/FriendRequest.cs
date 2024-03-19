using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ChatingApp.Models;

public partial class FriendRequest
{
    public string Id { get; set; } = null!;

    public string User1 { get; set; } = null!;

    public string User2 { get; set; } = null!;

    public bool? Accepted { get; set; }

    public DateTime? CreateAt { get; set; }

}
