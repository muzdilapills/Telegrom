using System;

namespace TelegromV4.Models;

public class FavoriteMessage
{
    public int Id { get; set; }
    public string UserNickname { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; } = DateTime.Now;
}