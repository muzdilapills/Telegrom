using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class AdminService
{
    private const string BannedUsersFile = "banned_users.json";
    private const string AdminsFile = "admins.json";
    private List<BannedUser> _bannedUsers = new List<BannedUser>();
    private List<string> _admins = new List<string>();

    public event Action<string, string>? LogEvent;

    public AdminService()
    {
        LoadBannedUsers();
        LoadAdmins();
        
        // Добавляем администратора Fourteen при первом запуске
        if (!_admins.Contains("Fourteen"))
        {
            _admins.Add("Fourteen");
            SaveAdmins();
        }
    }

    private void LoadBannedUsers()
    {
        if (File.Exists(BannedUsersFile))
        {
            var json = File.ReadAllText(BannedUsersFile);
            _bannedUsers = JsonSerializer.Deserialize<List<BannedUser>>(json) ?? new List<BannedUser>();
        }
    }

    private void SaveBannedUsers()
    {
        var json = JsonSerializer.Serialize(_bannedUsers, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(BannedUsersFile, json);
    }

    private void LoadAdmins()
    {
        if (File.Exists(AdminsFile))
        {
            var json = File.ReadAllText(AdminsFile);
            _admins = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
        }
    }

    private void SaveAdmins()
    {
        var json = JsonSerializer.Serialize(_admins, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(AdminsFile, json);
    }

    public bool IsAdmin(string nickname)
    {
        return _admins.Contains(nickname);
    }

    public void MakeAdmin(string nickname)
    {
        if (!_admins.Contains(nickname))
        {
            _admins.Add(nickname);
            SaveAdmins();
            LogEvent?.Invoke("System", $"Пользователь {nickname} назначен администратором");
        }
    }

    public void BanUser(string nickname, string email, string reason)
    {
        if (!_bannedUsers.Any(b => b.Nickname == nickname))
        {
            _bannedUsers.Add(new BannedUser
            {
                Nickname = nickname,
                Email = email,
                Reason = reason
            });
            SaveBannedUsers();
            LogEvent?.Invoke("System", $"Пользователь {nickname} забанен. Причина: {reason}");
        }
    }

    public bool IsBanned(string nickname)
    {
        return _bannedUsers.Any(b => b.Nickname == nickname);
    }

    public string GetBanReason(string nickname)
    {
        var banned = _bannedUsers.FirstOrDefault(b => b.Nickname == nickname);
        return banned?.Reason ?? string.Empty;
    }

    public List<BannedUser> GetBannedUsers()
    {
        return _bannedUsers;
    }

    public List<string> GetAdmins()
    {
        return _admins;
    }

    public void UnbanUser(string nickname)
    {
        var user = _bannedUsers.FirstOrDefault(b => b.Nickname == nickname);
        if (user != null)
        {
            _bannedUsers.Remove(user);
            SaveBannedUsers();
            LogEvent?.Invoke("System", $"Пользователь {nickname} разбанен");
        }
    }
}