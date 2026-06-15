using System.Collections.Generic;

namespace TelegromV4.Models;

public class Channel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string CreatorNickname { get; set; } = string.Empty;
    public string? AvatarPath { get; set; }
    public List<Message> Messages { get; set; } = new List<Message>();
    public List<ChannelMember> Members { get; set; } = new List<ChannelMember>();
    
    public string DisplayName => Name;
    public string MemberCount => $"{Members.Count} подписчиков";
}

public class ChannelMember
{
    public string Nickname { get; set; } = string.Empty;
    public ChannelRole Role { get; set; } = ChannelRole.Subscriber;
}

public enum ChannelRole
{
    Subscriber,
    Admin,
    Creator
}