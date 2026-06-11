using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class UserService
{
    private const string UsersFile = "users_data.json";
    private List<User> _users = new List<User>();

    public UserService()
    {
        LoadUsers();
    }

    private void LoadUsers()
    {
        if (File.Exists(UsersFile))
        {
            var json = File.ReadAllText(UsersFile);
            _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
        else
        {
            _users = new List<User>();
        }
    }

    private void SaveUsers()
    {
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(UsersFile, json);
    }

    public bool Register(string nickname, string email, string password)
    {
        if (_users.Any(u => u.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase)))
        {
            return false;
        }

        var newUser = new User
        {
            Nickname = nickname,
            Email = email,
            Password = password
        };

        _users.Add(newUser);
        SaveUsers();
        return true;
    }

    public User? Login(string nickname, string password)
    {
        return _users.FirstOrDefault(u => 
            u.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase) && 
            u.Password == password);
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }
}