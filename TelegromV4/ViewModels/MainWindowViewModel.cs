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
    private string? _currentUser;
    
    [ObservableProperty]
    private object? _currentView;
    
    [ObservableProperty]
    private bool _showBanOverlay;
    
    [ObservableProperty]
    private string _banMessage = string.Empty;

    public MainWindowViewModel()
    {
        _userService = new UserService();
        _chatService = new ChatService();
        _adminService = new AdminService();
        
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
            
            // Проверка на бан
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

    private void ShowChatsView()
    {
        var chatsView = new ChatsView();
        var chatsVm = new ChatsViewModel(_chatService, _userService, _adminService, _currentUser!);
        chatsView.DataContext = chatsVm;
        
        chatsVm.OpenCreateChatRequested += () => ShowCreateChatView();
        chatsVm.OpenTerminalRequested += () => ShowTerminalView();
        chatsVm.LogoutRequested += () => {
            _currentUser = null;
            ShowLoginView();
        };
chatsVm.OpenChatSettingsRequested += (settingsVm) => {
        var settingsView = new ChatSettingsView();
        settingsView.DataContext = settingsVm;
        // Здесь нужно показать оверлей с настройками
        // В ChatsViewModel уже есть ShowSettings
        chatsView.ShowChatSettings(settingsVm);
    };
        
        CurrentView = chatsView;
    }

    private void ShowCreateChatView()
    {
        var createChatView = new CreateChatView();
        var createChatVm = new CreateChatViewModel(_chatService, _currentUser!);
        createChatView.DataContext = createChatVm;
        
        createChatVm.ChatCreated += () => {
            ShowChatsView();
        };
        createChatVm.CancelRequested += () => {
            ShowChatsView();
        };
        
        CurrentView = createChatView;
    }

    private void ShowTerminalView()
    {
        var terminalView = new TerminalView();
        var terminalVm = new TerminalViewModel(_adminService, _userService, _chatService);
        terminalView.DataContext = terminalVm;
        
        terminalVm.CloseRequested += () => ShowChatsView();
        
        CurrentView = terminalView;
    }

    [RelayCommand]
    private void ExitFromBan()
    {
        ShowBanOverlay = false;
        ShowLoginView();
    }
}