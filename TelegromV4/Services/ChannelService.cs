using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class ChannelService
{
    private const string ChannelsFile = "channels_data.json";
    private List<Channel> _channels = new List<Channel>();
    private readonly LogService _logService;

    public ChannelService(LogService logService)
    {
        _logService = logService;
        LoadChannels();
    }

    private void LoadChannels()
    {
        if (File.Exists(ChannelsFile))
        {
            var json = File.ReadAllText(ChannelsFile);
            _channels = JsonSerializer.Deserialize<List<Channel>>(json) ?? new List<Channel>();
        }
    }

    private void SaveChannels()
    {
        var json = JsonSerializer.Serialize(_channels, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ChannelsFile, json);
    }

    public Channel CreateChannel(string name, string creatorNickname, string? avatarPath = null)
    {
        var channel = new Channel
        {
            Id = _channels.Count > 0 ? _channels.Max(c => c.Id) + 1 : 1,
            Name = name,
            CreatorNickname = creatorNickname,
            AvatarPath = avatarPath,
            Members = new List<ChannelMember>
            {
                new ChannelMember
                {
                    Nickname = creatorNickname,
                    Role = ChannelRole.Creator
                }
            },
            Messages = new List<Message>()
        };
        
        _channels.Add(channel);
        SaveChannels();
        
        _logService.AddLog(creatorNickname, "CreateChannel", name, $"Создан канал: {name}");
        return channel;
    }

    public List<Channel> GetUserChannels(string nickname)
    {
        return _channels.Where(c => c.Members.Any(m => m.Nickname == nickname)).ToList();
    }

    public Channel? GetChannel(int channelId)
    {
        return _channels.FirstOrDefault(c => c.Id == channelId);
    }

    public List<Channel> GetAllChannels()
    {
        return _channels;
    }

    public bool SubscribeToChannel(int channelId, string nickname)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return false;
        
        if (channel.Members.Any(m => m.Nickname == nickname)) return false;
        
        channel.Members.Add(new ChannelMember
        {
            Nickname = nickname,
            Role = ChannelRole.Subscriber
        });
        SaveChannels();
        _logService.AddLog(nickname, "SubscribeToChannel", channel.Name, "Подписался на канал");
        return true;
    }

    public bool UnsubscribeFromChannel(int channelId, string nickname)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return false;
        
        var member = channel.Members.FirstOrDefault(m => m.Nickname == nickname);
        if (member == null) return false;
        
        if (member.Role == ChannelRole.Creator) return false;
        
        channel.Members.Remove(member);
        SaveChannels();
        _logService.AddLog(nickname, "UnsubscribeFromChannel", channel.Name, "Отписался от канала");
        return true;
    }

    public bool CanSendToChannel(int channelId, string nickname)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return false;
        
        var member = channel.Members.FirstOrDefault(m => m.Nickname == nickname);
        return member != null && (member.Role == ChannelRole.Admin || member.Role == ChannelRole.Creator);
    }

    public bool MakeChannelAdmin(int channelId, string creatorNickname, string targetNickname)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return false;
        
        var creator = channel.Members.FirstOrDefault(m => m.Nickname == creatorNickname);
        if (creator?.Role != ChannelRole.Creator) return false;
        
        var target = channel.Members.FirstOrDefault(m => m.Nickname == targetNickname);
        if (target == null) return false;
        
        target.Role = ChannelRole.Admin;
        SaveChannels();
        _logService.AddLog(creatorNickname, "MakeChannelAdmin", channel.Name, $"Назначен администратор: {targetNickname}");
        return true;
    }

    public bool RemoveChannelAdmin(int channelId, string creatorNickname, string targetNickname)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return false;
        
        var creator = channel.Members.FirstOrDefault(m => m.Nickname == creatorNickname);
        if (creator?.Role != ChannelRole.Creator) return false;
        
        var target = channel.Members.FirstOrDefault(m => m.Nickname == targetNickname);
        if (target?.Role != ChannelRole.Admin) return false;
        
        target.Role = ChannelRole.Subscriber;
        SaveChannels();
        _logService.AddLog(creatorNickname, "RemoveChannelAdmin", channel.Name, $"Снят администратор: {targetNickname}");
        return true;
    }

    public void SendMessageToChannel(int channelId, string senderNickname, string content)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return;
        
        if (!CanSendToChannel(channelId, senderNickname)) return;
        
        var message = new Message
        {
            Id = channel.Messages.Count > 0 ? channel.Messages.Max(m => m.Id) + 1 : 1,
            SenderNickname = senderNickname,
            Content = content,
            Timestamp = DateTime.Now
        };
        
        channel.Messages.Add(message);
        SaveChannels();
        
        _logService.AddLog(senderNickname, "SendChannelMessage", channel.Name, content);
    }

    public List<Message> GetChannelMessages(int channelId)
    {
        var channel = GetChannel(channelId);
        return channel?.Messages ?? new List<Message>();
    }

    public bool DeleteChannel(int channelId, string creatorNickname)
    {
        var channel = GetChannel(channelId);
        if (channel == null) return false;
        
        if (channel.CreatorNickname != creatorNickname) return false;
        
        _channels.Remove(channel);
        SaveChannels();
        _logService.AddLog(creatorNickname, "DeleteChannel", channel.Name, "Канал удалён");
        return true;
    }
}