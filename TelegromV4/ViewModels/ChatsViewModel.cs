using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class ChatsViewModel : ObservableObject
{
    private readonly ChatService _chatService;
    private readonly UserService _userService;
    private readonly AdminService _adminService;
    private readonly ChannelService _channelService;
    private readonly PrivateChatService _privateChatService;
    private readonly ContactService _contactService;
    private readonly FavoriteService _favoriteService;
    private readonly string _currentUser;

    // Коллекции для отображения
    [ObservableProperty]
    private ObservableCollection<Chat> _chats = new ObservableCollection<Chat>();
    
    [ObservableProperty]
    private ObservableCollection<Channel> _channels = new ObservableCollection<Channel>();
    
    [ObservableProperty]
    private ObservableCollection<PrivateChat> _privateChats = new ObservableCollection<PrivateChat>();
    
    // Выбранный элемент (чат, канал или личный чат)
    [ObservableProperty]
    private object? _selectedChatItem;
    
    // Сообщения и ввод
    [ObservableProperty]
    private string _newMessage = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<Message> _messages = new ObservableCollection<Message>();
    
    // Состояния интерфейса
    [ObservableProperty]
    private bool _isAdmin;
    
    [ObservableProperty]
    private bool _showSettings;
    
    [ObservableProperty]
    private string _currentChatTitle = string.Empty;
    
    [ObservableProperty]
    private bool _canSendMessage = true;
    
    [ObservableProperty]
    private bool _isChatSelected;
    
    [ObservableProperty]
    private string _otherUser = string.Empty;
    
    // Профиль пользователя
    [ObservableProperty]
    private bool _showUserProfile;
    
    [ObservableProperty]
    private UserProfileViewModel? _userProfileViewModel;
    
    // Избранное
    [ObservableProperty]
    private bool _showFavorite;
    
    [ObservableProperty]
    private ObservableCollection<FavoriteMessage> _favoriteMessages = new ObservableCollection<FavoriteMessage>();
    
    [ObservableProperty]
    private string _newFavoriteMessage = string.Empty;
    [ObservableProperty]
    private bool _showChannelMembers;

    [ObservableProperty]
    private ChannelMembersViewModel? _channelMembersViewModel;

[ObservableProperty]
private bool _isChannelSelected;

    [RelayCommand]
    private void OpenChannelMembers()
    {
        if (SelectedChatItem is Channel channel)
        {
            var membersVm = new ChannelMembersViewModel(channel);
            membersVm.Closed += () => ShowChannelMembers = false;
            ChannelMembersViewModel = membersVm;
            ShowChannelMembers = true;
        }
    }

    // События для навигации
    public event Action? OpenCreateChatRequested;
    public event Action? OpenCreateGroupRequested;
    public event Action? OpenCreateChannelRequested;
    public event Action? OpenTerminalRequested;
    public event Action? OpenSettingsRequested;
    public event Action? OpenChatInfoRequested;
    public event Action? LogoutRequested;
    public event Action<ChatSettingsViewModel>? OpenChatSettingsRequested;

    // Конструктор
    public ChatsViewModel(ChatService chatService, UserService userService, AdminService adminService, 
                          ChannelService channelService, PrivateChatService privateChatService, 
                          ContactService contactService, FavoriteService favoriteService, string currentUser)
    {
        _chatService = chatService;
        _userService = userService;
        _adminService = adminService;
        _channelService = channelService;
        _privateChatService = privateChatService;
        _contactService = contactService;
        _favoriteService = favoriteService;
        _currentUser = currentUser;
        _isAdmin = _adminService.IsAdmin(currentUser);
        
        LoadAllChats();
    }

    // Загрузка всех чатов
    private void LoadAllChats()
    {
        LoadChats();
        LoadChannels();
        LoadPrivateChats();
    }

    // Загрузка групп
    private void LoadChats()
    {
        Chats.Clear();
        foreach (var chat in _chatService.GetUserChats(_currentUser))
        {
            Chats.Add(chat);
        }
    }

    // Загрузка каналов
    private void LoadChannels()
    {
        Channels.Clear();
        foreach (var channel in _channelService.GetUserChannels(_currentUser))
        {
            Channels.Add(channel);
        }
    }

    // Загрузка личных чатов
    private void LoadPrivateChats()
    {
        PrivateChats.Clear();
        foreach (var privateChat in _privateChatService.GetUserPrivateChats(_currentUser))
        {
            PrivateChats.Add(privateChat);
        }
    }

    // При изменении выбранного чата
    partial void OnSelectedChatItemChanged(object? value)
{
    IsChatSelected = value != null;
    IsChannelSelected = value is Channel;
    if (value != null)
    {
        LoadMessagesForSelectedItem();
    }
    else
    {
        Messages.Clear();
        CurrentChatTitle = "Выберите чат";
    }
}

    // Загрузка сообщений для выбранного элемента
    private void LoadMessagesForSelectedItem()
    {
        Messages.Clear();
        
        if (SelectedChatItem is Chat chat)
        {
            CurrentChatTitle = chat.Name;
            CanSendMessage = true;
            foreach (var msg in _chatService.GetChatMessages(chat.Id))
            {
                Messages.Add(msg);
            }
        }
        else if (SelectedChatItem is Channel channel)
        {
            CurrentChatTitle = $"📢 {channel.Name}";
            CanSendMessage = _channelService.CanSendToChannel(channel.Id, _currentUser);
            foreach (var msg in _channelService.GetChannelMessages(channel.Id))
            {
                Messages.Add(msg);
            }
        }
        else if (SelectedChatItem is PrivateChat privateChat)
        {
            OtherUser = privateChat.GetOtherUser(_currentUser);
            CurrentChatTitle = OtherUser;
            CanSendMessage = true;
            foreach (var msg in _privateChatService.GetPrivateMessages(privateChat.User1, privateChat.User2))
            {
                Messages.Add(msg);
            }
        }
    }

    // Отправка сообщения
    [RelayCommand]
    private void SendMessage()
    {
        if (string.IsNullOrWhiteSpace(NewMessage) || SelectedChatItem == null)
            return;
        
        if (SelectedChatItem is Chat chat)
        {
            _chatService.SendMessage(chat.Id, _currentUser, NewMessage);
        }
        else if (SelectedChatItem is Channel channel)
        {
            if (_channelService.CanSendToChannel(channel.Id, _currentUser))
            {
                _channelService.SendMessageToChannel(channel.Id, _currentUser, NewMessage);
            }
        }
        else if (SelectedChatItem is PrivateChat privateChat)
        {
            var otherUser = privateChat.GetOtherUser(_currentUser);
            _privateChatService.SendPrivateMessage(_currentUser, otherUser, NewMessage);
        }
        
        LoadMessagesForSelectedItem();
        NewMessage = string.Empty;
    }

    // Открыть настройки чата
    [RelayCommand]
    private void OpenChatSettings()
    {
        if (SelectedChatItem is Chat chat)
        {
            var settingsVm = new ChatSettingsViewModel(_chatService, _userService, chat, _currentUser);
            settingsVm.ChatUpdated += () => {
                LoadAllChats();
                LoadMessagesForSelectedItem();
            };
            settingsVm.SettingsClosed += () => {
                ShowSettings = false;
                LoadAllChats();
                if (SelectedChatItem == null || !_chatService.GetUserChats(_currentUser).Contains(chat))
                {
                    SelectedChatItem = null;
                }
            };
            OpenChatSettingsRequested?.Invoke(settingsVm);
            ShowSettings = true;
        }
    }

    // Закрыть настройки
    [RelayCommand]
    private void CloseSettings()
    {
        ShowSettings = false;
    }

    // Открыть создание группы
    [RelayCommand]
    private void OpenCreateGroup()
    {
        OpenCreateGroupRequested?.Invoke();
    }

    // Открыть создание канала
    [RelayCommand]
    private void OpenCreateChannel()
    {
        OpenCreateChannelRequested?.Invoke();
    }

    // Открыть создание чата (устаревшее, оставлено для совместимости)
    [RelayCommand]
    private void OpenCreateChat()
    {
        OpenCreateChatRequested?.Invoke();
    }

    // Открыть терминал (только для админов)
    [RelayCommand]
    private void OpenTerminal()
    {
        if (IsAdmin)
        {
            OpenTerminalRequested?.Invoke();
        }
    }

    // Открыть настройки приложения
    [RelayCommand]
    private void OpenSettings()
    {
        OpenSettingsRequested?.Invoke();
    }

    // Открыть информацию о чате
    [RelayCommand]
    private void OpenChatInfo()
    {
        OpenChatInfoRequested?.Invoke();
    }

    // Выйти из аккаунта
    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }

    // Открыть профиль пользователя
    [RelayCommand]
    private void OpenUserProfile(string nickname)
    {
        var profileVm = new UserProfileViewModel(_userService, _contactService, _privateChatService, _currentUser, nickname);
        profileVm.StartPrivateChatRequested += (targetUser) => {
            StartPrivateChat(targetUser);
            ShowUserProfile = false;
        };
        profileVm.ProfileClosed += () => {
            ShowUserProfile = false;
        };
        UserProfileViewModel = profileVm;
        ShowUserProfile = true;
    }

    // Начать личный чат
    private void StartPrivateChat(string targetUser)
    {
        var chat = _privateChatService.GetOrCreatePrivateChat(_currentUser, targetUser);
        LoadAllChats();
        SelectedChatItem = chat;
    }

    // Добавить в избранное
    [RelayCommand]
    private void AddToFavorites()
    {
        if (!string.IsNullOrWhiteSpace(NewFavoriteMessage))
        {
            _favoriteService.AddToFavorites(_currentUser, NewFavoriteMessage);
            LoadFavorites();
            NewFavoriteMessage = string.Empty;
        }
    }

    // Открыть избранное
    [RelayCommand]
    private void OpenFavorite()
    {
        LoadFavorites();
        ShowFavorite = true;
    }

    // Загрузка избранного
    private void LoadFavorites()
    {
        FavoriteMessages.Clear();
        foreach (var fav in _favoriteService.GetUserFavorites(_currentUser))
        {
            FavoriteMessages.Add(fav);
        }
    }

    // Закрыть избранное
    [RelayCommand]
    private void CloseFavorite()
    {
        ShowFavorite = false;
    }

    // Закрыть профиль пользователя
    [RelayCommand]
    private void CloseUserProfile()
    {
        ShowUserProfile = false;
    }
}