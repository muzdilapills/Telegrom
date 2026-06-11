using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class ChatsViewModel : ObservableObject
{
    private readonly ChatService _chatService;
    private readonly UserService _userService;
    private readonly AdminService _adminService;
    private readonly string _currentUser;

    [ObservableProperty]
    private ObservableCollection<Chat> _chats = new ObservableCollection<Chat>();
    
    [ObservableProperty]
    private Chat? _selectedChat;
    
    [ObservableProperty]
    private string _newMessage = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
    
    [ObservableProperty]
    private bool _isAdmin;
    
    [ObservableProperty]
    private bool _showSettings;

    public event Action? OpenCreateChatRequested;
    public event Action? OpenTerminalRequested;
    public event Action? LogoutRequested;
    public event Action<ChatSettingsViewModel>? OpenChatSettingsRequested;

    public ChatsViewModel(ChatService chatService, UserService userService, AdminService adminService, string currentUser)
    {
        _chatService = chatService;
        _userService = userService;
        _adminService = adminService;
        _currentUser = currentUser;
        _isAdmin = _adminService.IsAdmin(currentUser);
        
        LoadChats();
    }

    private void LoadChats()
    {
        Chats.Clear();
        foreach (var chat in _chatService.GetUserChats(_currentUser))
        {
            Chats.Add(chat);
        }
    }

    partial void OnSelectedChatChanged(Chat? value)
    {
        if (value != null)
        {
            LoadMessages();
        }
    }

    private void LoadMessages()
    {
        Messages.Clear();
        if (SelectedChat != null)
        {
            foreach (var msg in _chatService.GetChatMessages(SelectedChat.Id))
            {
                Messages.Add(msg);
            }
        }
    }

    [RelayCommand]
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage) || SelectedChat == null)
            return;
        
        _chatService.SendMessage(SelectedChat.Id, _currentUser, NewMessage);
        LoadMessages();
        NewMessage = string.Empty;
    }

    [RelayCommand]
    private void OpenChatSettings()
    {
        if (SelectedChat != null)
        {
            var settingsVm = new ChatSettingsViewModel(_chatService, _userService, SelectedChat, _currentUser);
            settingsVm.ChatUpdated += () => {
                LoadChats();
                LoadMessages();
            };
            settingsVm.SettingsClosed += () => {
                ShowSettings = false;
                LoadChats(); // Обновляем список чатов после изменений
                if (SelectedChat == null || !_chatService.GetUserChats(_currentUser).Contains(SelectedChat))
                {
                    SelectedChat = null; // Если чат удалён или пользователь вышел
                }
            };
            OpenChatSettingsRequested?.Invoke(settingsVm);
            ShowSettings = true;
        }
    }

    [RelayCommand]
    private void CloseSettings()
    {
        ShowSettings = false;
    }

    [RelayCommand]
    private void OpenCreateChat()
    {
        OpenCreateChatRequested?.Invoke();
    }

    [RelayCommand]
    private void OpenTerminal()
    {
        if (IsAdmin)
        {
            OpenTerminalRequested?.Invoke();
        }
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }
}