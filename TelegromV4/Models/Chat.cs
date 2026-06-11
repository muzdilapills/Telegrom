using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TelegromV4.Models;

public class Chat
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatorNickname { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
    public List<ChatMember> Members { get; set; } = new List<ChatMember>();
    
    // Для отображения в UI
    public string DisplayName => Name;
    public string MemberCount => $"{Members.Count} участников";
}

public class ChatMember
{
    public string Nickname { get; set; } = string.Empty;
    public ChatRole Role { get; set; } = ChatRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.Now;
}

public enum ChatRole
{
    Member,
    Admin,
    Creator
}