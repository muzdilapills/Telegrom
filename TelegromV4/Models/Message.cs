using System;

namespace TelegromV4.Models;

public class Message
{
    public int Id { get; set; }
    public string SenderNickname { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}