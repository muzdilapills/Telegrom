using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class TerminalViewModel : ObservableObject
{
    private readonly AdminService _adminService;
    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly ChannelService _channelService;
    private readonly LogService _logService;

    [ObservableProperty]
    private ObservableCollection<LogEntry> _logs = new ObservableCollection<LogEntry>();
    
    [ObservableProperty]
    private ObservableCollection<User> _allUsers = new ObservableCollection<User>();
    
    [ObservableProperty]
    private ObservableCollection<Chat> _allChats = new ObservableCollection<Chat>();
    
    [ObservableProperty]
    private ObservableCollection<Channel> _allChannels = new ObservableCollection<Channel>();
    
    [ObservableProperty]
    private User? _selectedUser;
    
    [ObservableProperty]
    private Chat? _selectedChat;
    
    [ObservableProperty]
    private Channel? _selectedChannel;
    
    [ObservableProperty]
    private string _banReason = string.Empty;
    
    [ObservableProperty]
    private string _selectedUserInfo = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<string> _chatMembers = new ObservableCollection<string>();
    
    [ObservableProperty]
    private ObservableCollection<string> _chatAdmins = new ObservableCollection<string>();
    
    [ObservableProperty]
    private string _selectedTab = "Users";

    public event Action? CloseRequested;

    public TerminalViewModel(AdminService adminService, UserService userService, 
                             ChatService chatService, ChannelService channelService, LogService logService)
    {
        _adminService = adminService;
        _userService = userService;
        _chatService = chatService;
        _channelService = channelService;
        _logService = logService;
        
        LoadUsers();
        LoadChats();
        LoadChannels();
        LoadLogs();
    }

    private void LoadUsers()
    {
        AllUsers.Clear();
        foreach (var user in _userService.GetAllUsers())
        {
            AllUsers.Add(user);
        }
    }

    private void LoadChats()
    {
        AllChats.Clear();
        foreach (var chat in _chatService.GetAllChats())
        {
            AllChats.Add(chat);
        }
    }

    private void LoadChannels()
    {
        AllChannels.Clear();
        foreach (var channel in _channelService.GetAllChannels())
        {
            AllChannels.Add(channel);
        }
    }

    private void LoadLogs()
    {
        Logs.Clear();
        foreach (var log in _logService.GetAllLogs())
        {
            Logs.Add(log);
        }
    }

    partial void OnSelectedUserChanged(User? value)
    {
        if (value != null)
        {
            SelectedUserInfo = $"Никнейм: {value.Nickname}\nEmail: {value.Email}\nАдмин: {(_adminService.IsAdmin(value.Nickname) ? "Да" : "Нет")}\nЗабанен: {(_adminService.IsBanned(value.Nickname) ? "Да" : "Нет")}\nПричина бана: {_adminService.GetBanReason(value.Nickname)}";
        }
    }

    partial void OnSelectedChatChanged(Chat? value)
    {
        if (value != null)
        {
            ChatMembers.Clear();
            ChatAdmins.Clear();
            foreach (var member in value.Members)
            {
                ChatMembers.Add($"{member.Nickname} ({(member.Role == ChatRole.Creator ? "Создатель" : (member.Role == ChatRole.Admin ? "Админ" : "Участник"))})");
                if (member.Role == ChatRole.Admin || member.Role == ChatRole.Creator)
                {
                    ChatAdmins.Add(member.Nickname);
                }
            }
        }
    }

    [RelayCommand]
    private void MakeAdmin()
    {
        if (SelectedUser != null)
        {
            _adminService.MakeAdmin(SelectedUser.Nickname);
            LoadUsers();
            LoadLogs();
            OnSelectedUserChanged(SelectedUser);
        }
    }

    [RelayCommand]
    private void RemoveAdmin()
    {
        if (SelectedUser != null)
        {
            _adminService.RemoveAdmin(SelectedUser.Nickname);
            LoadUsers();
            LoadLogs();
            OnSelectedUserChanged(SelectedUser);
        }
    }

    [RelayCommand]
    private void BanUser()
    {
        if (SelectedUser != null && !string.IsNullOrWhiteSpace(BanReason))
        {
            _adminService.BanUser(SelectedUser.Nickname, SelectedUser.Email, BanReason);
            LoadUsers();
            LoadLogs();
            BanReason = string.Empty;
            OnSelectedUserChanged(SelectedUser);
        }
    }

    [RelayCommand]
    private void UnbanUser()
    {
        if (SelectedUser != null)
        {
            _adminService.UnbanUser(SelectedUser.Nickname);
            LoadUsers();
            LoadLogs();
            OnSelectedUserChanged(SelectedUser);
        }
    }

    [RelayCommand]
    private void JoinChatAsAdmin()
    {
        if (SelectedChat != null)
        {
            _chatService.AddMemberToChat(SelectedChat.Id, "admin", "admin");
            _chatService.MakeAdmin(SelectedChat.Id, SelectedChat.CreatorNickname, "admin");
            LoadChats();
            OnSelectedChatChanged(SelectedChat);
            _logService.AddLog("admin", "JoinChatAsAdmin", SelectedChat.Name, "Администратор присоединился к чату");
        }
    }

    [RelayCommand]
    private void DeleteChat()
    {
        if (SelectedChat != null)
        {
            var chatName = SelectedChat.Name;
            _chatService.DeleteChat(SelectedChat.Id, SelectedChat.CreatorNickname);
            LoadChats();
            SelectedChat = null;
            _logService.AddLog("admin", "DeleteChatFromAdmin", chatName, "Чат удалён администратором");
        }
    }

    [RelayCommand]
    private void JoinChannelAsAdmin()
    {
        if (SelectedChannel != null)
        {
            _channelService.SubscribeToChannel(SelectedChannel.Id, "admin");
            _channelService.MakeChannelAdmin(SelectedChannel.Id, SelectedChannel.CreatorNickname, "admin");
            LoadChannels();
            _logService.AddLog("admin", "JoinChannelAsAdmin", SelectedChannel.Name, "Администратор присоединился к каналу");
        }
    }

    [RelayCommand]
    private void DeleteChannel()
    {
        if (SelectedChannel != null)
        {
            var channelName = SelectedChannel.Name;
            _channelService.DeleteChannel(SelectedChannel.Id, SelectedChannel.CreatorNickname);
            LoadChannels();
            SelectedChannel = null;
            _logService.AddLog("admin", "DeleteChannelFromAdmin", channelName, "Канал удалён администратором");
        }
    }

    [RelayCommand]
    private void Refresh()
    {
        LoadUsers();
        LoadChats();
        LoadChannels();
        LoadLogs();
    }

    [RelayCommand]
    private void SetUsersTab()
    {
        SelectedTab = "Users";
    }

    [RelayCommand]
    private void SetChatsTab()
    {
        SelectedTab = "Chats";
    }

    [RelayCommand]
    private void SetChannelsTab()
    {
        SelectedTab = "Channels";
    }

    [RelayCommand]
    private void SetLogsTab()
    {
        SelectedTab = "Logs";
    }

    [RelayCommand]
    private void Close()
    {
        CloseRequested?.Invoke();
    }
}