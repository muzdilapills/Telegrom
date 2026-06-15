using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class PrivateChatService
{
    private const string PrivateChatsFile = "userstheirchats_base.json";
    private List<PrivateChat> _privateChats = new List<PrivateChat>();
    private int _nextId = 1;

    public PrivateChatService()
    {
        LoadPrivateChats();
    }

    private void LoadPrivateChats()
    {
        if (File.Exists(PrivateChatsFile))
        {
            var json = File.ReadAllText(PrivateChatsFile);
            _privateChats = JsonSerializer.Deserialize<List<PrivateChat>>(json) ?? new List<PrivateChat>();
            if (_privateChats.Any())
                _nextId = _privateChats.Max(c => c.Id) + 1;
        }
    }

    private void SavePrivateChats()
    {
        var json = JsonSerializer.Serialize(_privateChats, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(PrivateChatsFile, json);
    }

    public PrivateChat GetOrCreatePrivateChat(string user1, string user2)
    {
        var chat = _privateChats.FirstOrDefault(c => 
            (c.User1 == user1 && c.User2 == user2) || 
            (c.User1 == user2 && c.User2 == user1));
        
        if (chat == null)
        {
            chat = new PrivateChat
            {
                Id = _nextId++,
                User1 = user1,
                User2 = user2,
                Messages = new List<Message>()
            };
            _privateChats.Add(chat);
            SavePrivateChats();
        }
        return chat;
    }

    public List<PrivateChat> GetUserPrivateChats(string nickname)
    {
        return _privateChats.Where(c => c.User1 == nickname || c.User2 == nickname).ToList();
    }

    public void SendPrivateMessage(string fromUser, string toUser, string content)
    {
        var chat = GetOrCreatePrivateChat(fromUser, toUser);
        var message = new Message
        {
            Id = chat.Messages.Count > 0 ? chat.Messages.Max(m => m.Id) + 1 : 1,
            SenderNickname = fromUser,
            Content = content,
            Timestamp = DateTime.Now
        };
        chat.Messages.Add(message);
        SavePrivateChats();
    }

    public List<Message> GetPrivateMessages(string user1, string user2)
    {
        var chat = GetOrCreatePrivateChat(user1, user2);
        return chat.Messages;
    }
}