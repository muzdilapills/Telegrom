using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class ChatService
{
    private const string ChatsFile = "chats_data.json";
    private List<Chat> _chats = new List<Chat>();

    public event Action<string, string>? LogEvent;

    public ChatService()
    {
        LoadChats();
    }

    private void LoadChats()
    {
        if (File.Exists(ChatsFile))
        {
            var json = File.ReadAllText(ChatsFile);
            _chats = JsonSerializer.Deserialize<List<Chat>>(json) ?? new List<Chat>();
        }
    }

    private void SaveChats()
    {
        var json = JsonSerializer.Serialize(_chats, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(ChatsFile, json);
    }

    public Chat CreateChat(string name, string creatorNickname)
    {
        var chat = new Chat
        {
            Id = _chats.Count > 0 ? _chats.Max(c => c.Id) + 1 : 1,
            Name = name,
            CreatorNickname = creatorNickname,
            Members = new List<ChatMember>
            {
                new ChatMember
                {
                    Nickname = creatorNickname,
                    Role = ChatRole.Creator,
                    JoinedAt = DateTime.Now
                }
            },
            Messages = new List<Message>()
        };
        
        _chats.Add(chat);
        SaveChats();
        
        LogEvent?.Invoke(creatorNickname, $"Создал чат: {name}");
        return chat;
    }

    public List<Chat> GetUserChats(string nickname)
    {
        return _chats.Where(c => c.Members.Any(m => m.Nickname == nickname)).ToList();
    }

    public Chat? GetChat(int chatId)
    {
        return _chats.FirstOrDefault(c => c.Id == chatId);
    }

    public bool AddMemberToChat(int chatId, string adminNickname, string newMemberNickname)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var admin = chat.Members.FirstOrDefault(m => m.Nickname == adminNickname);
        if (admin == null || (admin.Role != ChatRole.Admin && admin.Role != ChatRole.Creator))
            return false;
        
        if (chat.Members.Any(m => m.Nickname == newMemberNickname))
            return false;
        
        chat.Members.Add(new ChatMember
        {
            Nickname = newMemberNickname,
            Role = ChatRole.Member,
            JoinedAt = DateTime.Now
        });
        
        SaveChats();
        LogEvent?.Invoke(adminNickname, $"Добавил {newMemberNickname} в чат {chat.Name}");
        return true;
    }

    public bool RemoveMemberFromChat(int chatId, string adminNickname, string memberNickname)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var admin = chat.Members.FirstOrDefault(m => m.Nickname == adminNickname);
        if (admin == null || (admin.Role != ChatRole.Admin && admin.Role != ChatRole.Creator))
            return false;
        
        var member = chat.Members.FirstOrDefault(m => m.Nickname == memberNickname);
        if (member == null || member.Role == ChatRole.Creator) return false;
        
        chat.Members.Remove(member);
        SaveChats();
        LogEvent?.Invoke(adminNickname, $"Удалил {memberNickname} из чата {chat.Name}");
        return true;
    }

    public bool MakeAdmin(int chatId, string creatorNickname, string targetNickname)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var creator = chat.Members.FirstOrDefault(m => m.Nickname == creatorNickname);
        if (creator == null || creator.Role != ChatRole.Creator) return false;
        
        var target = chat.Members.FirstOrDefault(m => m.Nickname == targetNickname);
        if (target == null) return false;
        
        target.Role = ChatRole.Admin;
        SaveChats();
        LogEvent?.Invoke(creatorNickname, $"Назначил {targetNickname} администратором чата {chat.Name}");
        return true;
    }

    public bool RemoveAdmin(int chatId, string creatorNickname, string targetNickname)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var creator = chat.Members.FirstOrDefault(m => m.Nickname == creatorNickname);
        if (creator == null || creator.Role != ChatRole.Creator) return false;
        
        var target = chat.Members.FirstOrDefault(m => m.Nickname == targetNickname);
        if (target == null || target.Role != ChatRole.Admin) return false;
        
        target.Role = ChatRole.Member;
        SaveChats();
        LogEvent?.Invoke(creatorNickname, $"Снял администратора {targetNickname} с чата {chat.Name}");
        return true;
    }

    public bool UpdateChatName(int chatId, string adminNickname, string newName)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var admin = chat.Members.FirstOrDefault(m => m.Nickname == adminNickname);
        if (admin == null || (admin.Role != ChatRole.Admin && admin.Role != ChatRole.Creator))
            return false;
        
        var oldName = chat.Name;
        chat.Name = newName;
        SaveChats();
        LogEvent?.Invoke(adminNickname, $"Переименовал чат '{oldName}' в '{newName}'");
        return true;
    }

    public bool UpdateChatAvatar(int chatId, string adminNickname, string avatarPath)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var admin = chat.Members.FirstOrDefault(m => m.Nickname == adminNickname);
        if (admin == null || (admin.Role != ChatRole.Admin && admin.Role != ChatRole.Creator))
            return false;
        
        chat.AvatarPath = avatarPath;
        SaveChats();
        LogEvent?.Invoke(adminNickname, $"Изменил аватар чата {chat.Name}");
        return true;
    }

    public bool DeleteChat(int chatId, string creatorNickname)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        if (chat.CreatorNickname != creatorNickname) return false;
        
        _chats.Remove(chat);
        SaveChats();
        LogEvent?.Invoke(creatorNickname, $"Удалил чат {chat.Name}");
        return true;
    }

    public bool LeaveChat(int chatId, string nickname)
    {
        var chat = GetChat(chatId);
        if (chat == null) return false;
        
        var member = chat.Members.FirstOrDefault(m => m.Nickname == nickname);
        if (member == null) return false;
        
        if (member.Role == ChatRole.Creator)
        {
            // Создатель не может выйти, только удалить чат
            return false;
        }
        
        chat.Members.Remove(member);
        SaveChats();
        LogEvent?.Invoke(nickname, $"Покинул чат {chat.Name}");
        return true;
    }

    public void SendMessage(int chatId, string senderNickname, string content)
    {
        var chat = GetChat(chatId);
        if (chat == null) return;
        
        if (!chat.Members.Any(m => m.Nickname == senderNickname)) return;
        
        var message = new Message
        {
            Id = chat.Messages.Count > 0 ? chat.Messages.Max(m => m.Id) + 1 : 1,
            SenderNickname = senderNickname,
            Content = content,
            Timestamp = DateTime.Now
        };
        
        chat.Messages.Add(message);
        SaveChats();
        
        LogEvent?.Invoke(senderNickname, $"Отправил сообщение в чат {chat.Name}: {content}");
    }

    public List<Message> GetChatMessages(int chatId)
    {
        var chat = GetChat(chatId);
        return chat?.Messages ?? new List<Message>();
    }
    
    public List<User> GetAvailableUsersToAdd(int chatId, List<User> allUsers)
    {
        var chat = GetChat(chatId);
        if (chat == null) return new List<User>();
        
        var memberNicknames = chat.Members.Select(m => m.Nickname).ToList();
        return allUsers.Where(u => !memberNicknames.Contains(u.Nickname)).ToList();
    }
}