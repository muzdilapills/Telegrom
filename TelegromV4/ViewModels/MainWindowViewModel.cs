using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Services;
using TelegromV4.Views;

namespace TelegromV4.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly UserService _userService;
    private readonly ChatService _chatService;
    private readonly AdminService _adminService;
    private readonly LogService _logService;
    private readonly SettingsService _settingsService;
    private readonly ContactService _contactService;
    private readonly FavoriteService _favoriteService;
    private readonly PrivateChatService _privateChatService;
    private readonly ChannelService _channelService;
    private string? _currentUser;
    
    [ObservableProperty]
    private object? _currentView;
    
    [ObservableProperty]
    private bool _showBanOverlay;
    
    [ObservableProperty]
    private string _banMessage = string.Empty;

    public MainWindowViewModel()
    {
        _logService = new LogService();
        _userService = new UserService(_logService);
        _adminService = new AdminService(_logService);
        _chatService = new ChatService(_logService);
        _settingsService = new SettingsService();
        _contactService = new ContactService();
        _favoriteService = new FavoriteService();
        _privateChatService = new PrivateChatService();
        _channelService = new ChannelService(_logService);
        
        ShowLoginView();
    }

    private void ShowLoginView()
    {
        var loginView = new LoginView();
        var loginVm = new LoginViewModel(_userService, _adminService);
        loginView.DataContext = loginVm;
        
        loginVm.RegisterRequested += ShowRegisterView;
        loginVm.LoginSuccess += (user) => {
            _currentUser = user;
            
            if (_adminService.IsBanned(user))
            {
                var banReason = _adminService.GetBanReason(user);
                ShowBanOverlay = true;
                BanMessage = $"Вы забанены администратором!\nПричина: {banReason}\n\nНажмите кнопку ниже для выхода.";
            }
            else
            {
                ShowChatsView();
            }
        };
        
        CurrentView = loginView;
    }

    private void ShowRegisterView()
    {
        var registerView = new RegisterView();
        var registerVm = new RegisterViewModel(_userService);
        registerView.DataContext = registerVm;
        
        registerVm.RegisterSuccess += () => ShowLoginView();
        registerVm.LoginRequested += () => ShowLoginView();
        
        CurrentView = registerView;
    }
    [RelayCommand]
    private void Escape()
    {
        if (ShowBanOverlay)
        {
            ShowBanOverlay = false;
            ShowLoginView();
        }
        else if (CurrentView is ChatsView chatsView && chatsView.DataContext is ChatsViewModel chatsVm)
        {
            if (chatsVm.ShowSettings)
                chatsVm.CloseSettingsCommand.Execute(null);
            else if (chatsVm.ShowUserProfile)
                chatsVm.CloseUserProfileCommand.Execute(null);
            else if (chatsVm.ShowFavorite)
                chatsVm.CloseFavoriteCommand.Execute(null);
            else if (chatsVm.ShowChannelMembers)
                chatsVm.ShowChannelMembers = false;
            else
                chatsVm.SelectedChatItem = null;
        }
    }
    private void ShowChatsView()
{
    var chatsView = new ChatsView();
    var chatsVm = new ChatsViewModel(_chatService, _userService, _adminService, _channelService, _privateChatService, _contactService, _favoriteService, _currentUser!);
    chatsView.DataContext = chatsVm;
    
    chatsVm.OpenCreateChatRequested += () => ShowCreateChatView();
    chatsVm.OpenCreateGroupRequested += () => ShowCreateGroupView();
    chatsVm.OpenCreateChannelRequested += () => ShowCreateChannelView();
    chatsVm.OpenTerminalRequested += () => ShowTerminalView();
    chatsVm.OpenSettingsRequested += () => ShowSettingsView();
    chatsVm.LogoutRequested += () => {
        _currentUser = null;
        ShowLoginView();
    };
    
    CurrentView = chatsView;
}

    private void ShowCreateChatView()
    {
        var createChatView = new CreateChatView();
        var createChatVm = new CreateChatViewModel(_chatService, _currentUser!);
        createChatView.DataContext = createChatVm;
        
        createChatVm.ChatCreated += () => ShowChatsView();
        createChatVm.CancelRequested += () => ShowChatsView();
        
        CurrentView = createChatView;
    }

    private void ShowCreateGroupView()
    {
        var createGroupView = new CreateGroupView();
        var createGroupVm = new CreateGroupViewModel(_chatService, _userService, _currentUser!);
        createGroupView.DataContext = createGroupVm;
        
        createGroupVm.GroupCreated += () => ShowChatsView();
        createGroupVm.CancelRequested += () => ShowChatsView();
        
        CurrentView = createGroupView;
    }

    private void ShowCreateChannelView()
    {
        var createChannelView = new CreateChannelView();
        var createChannelVm = new CreateChannelViewModel(_channelService, _userService, _currentUser!);
        createChannelView.DataContext = createChannelVm;
        
        createChannelVm.ChannelCreated += () => ShowChatsView();
        createChannelVm.CancelRequested += () => ShowChatsView();
        
        CurrentView = createChannelView;
    }

    private void ShowTerminalView()
    {
        var terminalView = new TerminalView();
        var terminalVm = new TerminalViewModel(_adminService, _userService, _chatService, _channelService, _logService);
        terminalView.DataContext = terminalVm;
        
        terminalVm.CloseRequested += () => ShowChatsView();
        
        CurrentView = terminalView;
    }

    private void ShowSettingsView()
{
    var settingsView = new SettingsView();
    var settingsVm = new SettingsViewModel(_userService, _settingsService, _contactService, _currentUser!);
    settingsView.DataContext = settingsVm;
    
    settingsVm.SettingsClosed += () => ShowChatsView();
    settingsVm.LogoutRequested += () => {
        _currentUser = null;
        ShowLoginView();
    };
    settingsVm.ExitAppRequested += () => {
        Environment.Exit(0);
    };
    
    CurrentView = settingsView;
}

    [RelayCommand]
    private void ExitFromBan()
    {
        ShowBanOverlay = false;
        ShowLoginView();
    }
}