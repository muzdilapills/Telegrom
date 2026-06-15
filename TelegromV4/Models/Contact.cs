using System.Collections.Generic;

namespace TelegromV4.Models;

public class Contact
{
    public string OwnerNickname { get; set; } = string.Empty;
    public string ContactNickname { get; set; } = string.Empty;
    public string CustomName { get; set; } = string.Empty;
    public bool IsFavorite { get; set; }
}