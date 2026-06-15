using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class UserService
{
    private const string UsersFile = "users_data.json";
    private List<User> _users = new List<User>();
    private readonly LogService _logService;

    public UserService(LogService logService)
    {
        _logService = logService;
        LoadUsers();
        
        // Создаём администратора admin
        if (!_users.Any(u => u.Nickname == "admin"))
        {
            _users.Add(new User
            {
                Nickname = "admin",
                Email = "admin@gmail.com",
                Password = "1234"
            });
            SaveUsers();
            _logService.AddLog("System", "CreateAdmin", "admin", "Администратор admin создан");
        }
    }

    private void LoadUsers()
    {
        if (File.Exists(UsersFile))
        {
            var json = File.ReadAllText(UsersFile);
            _users = JsonSerializer.Deserialize<List<User>>(json) ?? new List<User>();
        }
    }

    private void SaveUsers()
    {
        var json = JsonSerializer.Serialize(_users, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(UsersFile, json);
    }

    public bool IsNicknameValid(string nickname)
    {
        // Только латиница, цифры и нижнее подчёркивание
        return Regex.IsMatch(nickname, @"^[a-zA-Z0-9_]+$");
    }

    public bool Register(string nickname, string email, string password)
    {
        if (!IsNicknameValid(nickname))
        {
            return false; // Никнейм содержит недопустимые символы
        }
        
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
        _logService.AddLog(nickname, "Register", "", "Пользователь зарегистрирован");
        return true;
    }

    public User? Login(string nickname, string password)
    {
        var user = _users.FirstOrDefault(u => 
            u.Nickname.Equals(nickname, StringComparison.OrdinalIgnoreCase) && 
            u.Password == password);
        
        if (user != null)
        {
            _logService.AddLog(nickname, "Login", "", "Вход в аккаунт");
        }
        
        return user;
    }

    public List<User> GetAllUsers()
    {
        return _users;
    }

    public User? GetUserByNickname(string nickname)
    {
        return _users.FirstOrDefault(u => u.Nickname == nickname);
    }
}