using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class TerminalViewModel : ObservableObject
{
    private readonly AdminService _adminService;
    private readonly UserService _userService;
    private readonly ChatService _chatService;

    [ObservableProperty]
    private ObservableCollection<LogEntry> _logs = new ObservableCollection<LogEntry>();
    
    [ObservableProperty]
    private ObservableCollection<User> _allUsers = new ObservableCollection<User>();
    
    [ObservableProperty]
    private User? _selectedUser;
    
    [ObservableProperty]
    private string _banReason = string.Empty;
    
    [ObservableProperty]
    private string _selectedUserInfo = string.Empty;

    public event Action? CloseRequested;

    public TerminalViewModel(AdminService adminService, UserService userService, ChatService chatService)
    {
        _adminService = adminService;
        _userService = userService;
        _chatService = chatService;
        
        LoadUsers();
        
        // Подписываемся на логи
        _adminService.LogEvent += OnLogEvent;
        _chatService.LogEvent += OnLogEvent;
    }

    private void OnLogEvent(string user, string action)
    {
        Logs.Insert(0, new LogEntry
        {
            Timestamp = DateTime.Now,
            User = user,
            Action = action
        });
    }

    private void LoadUsers()
    {
        AllUsers.Clear();
        foreach (var user in _userService.GetAllUsers())
        {
            AllUsers.Add(user);
        }
    }

    partial void OnSelectedUserChanged(User? value)
    {
        if (value != null)
        {
            SelectedUserInfo = $"Никнейм: {value.Nickname}\nEmail: {value.Email}\nАдмин: {_adminService.IsAdmin(value.Nickname)}\nЗабанен: {_adminService.IsBanned(value.Nickname)}";
        }
    }

    [RelayCommand]
    private void MakeAdmin()
    {
        if (SelectedUser != null)
        {
            _adminService.MakeAdmin(SelectedUser.Nickname);
            LoadUsers();
            SelectedUserInfo = $"Никнейм: {SelectedUser.Nickname}\nEmail: {SelectedUser.Email}\nАдмин: {_adminService.IsAdmin(SelectedUser.Nickname)}\nЗабанен: {_adminService.IsBanned(SelectedUser.Nickname)}";
        }
    }

    [RelayCommand]
    private void BanUser()
    {
        if (SelectedUser != null && !string.IsNullOrWhiteSpace(BanReason))
        {
            _adminService.BanUser(SelectedUser.Nickname, SelectedUser.Email, BanReason);
            LoadUsers();
            BanReason = string.Empty;
        }
    }

    [RelayCommand]
    private void ViewUserData()
    {
        if (SelectedUser != null)
        {
            SelectedUserInfo = $"Никнейм: {SelectedUser.Nickname}\nEmail: {SelectedUser.Email}\nАдмин: {_adminService.IsAdmin(SelectedUser.Nickname)}\nЗабанен: {_adminService.IsBanned(SelectedUser.Nickname)}\nПричина бана: {_adminService.GetBanReason(SelectedUser.Nickname)}";
        }
    }

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke();
    }
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string User { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
