using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly UserService _userService;
    private readonly SettingsService _settingsService;
    private readonly ContactService _contactService;
    private readonly string _currentUser;
    private UserSettings _userSettings;

    [ObservableProperty]
    private string _selectedTab = "Profile";

    // Профиль
    [ObservableProperty]
    private string _nickname = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _newNickname = string.Empty;
    
    [ObservableProperty]
    private string _newEmail = string.Empty;
    
    [ObservableProperty]
    private string _avatarPath = string.Empty;
    
    [ObservableProperty]
    private string _profileErrorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _profileHasError;

    // Оформление
    [ObservableProperty]
    private string[] _themes = new[] { "Светлая", "Темно-светлая", "Темно-мрачная", "Темно-красная" };
    
    [ObservableProperty]
    private int _selectedThemeIndex;
    
    [ObservableProperty]
    private string _wallpaperPath = string.Empty;

    // Приватность
    [ObservableProperty]
    private string[] _privacyOptions = new[] { "Все", "Только контакты", "Никто" };
    
    [ObservableProperty]
    private int _inviteToChatsIndex;
    
    [ObservableProperty]
    private int _inviteToChannelsIndex;
    
    [ObservableProperty]
    private int _privateMessagesIndex;
    
    [ObservableProperty]
    private int _showAvatarIndex;
    
    [ObservableProperty]
    private bool _streamerMode;

    // Контакты
    [ObservableProperty]
    private ObservableCollection<Contact> _contacts = new ObservableCollection<Contact>();
    
    [ObservableProperty]
    private Contact? _selectedContact;
    
    [ObservableProperty]
    private string _newContactName = string.Empty;
    
    [ObservableProperty]
    private string _contactErrorMessage = string.Empty;

    public event Action? SettingsClosed;
    public event Action? LogoutRequested;
    public event Action? ExitAppRequested;

    public SettingsViewModel(UserService userService, SettingsService settingsService, ContactService contactService, string currentUser)
    {
        _userService = userService;
        _settingsService = settingsService;
        _contactService = contactService;
        _currentUser = currentUser;
        
        var user = _userService.GetUserByNickname(currentUser);
        if (user != null)
        {
            Nickname = user.Nickname;
            Email = user.Email;
            AvatarPath = user.AvatarPath ?? "default_avatar.png";
        }
        
        _userSettings = _settingsService.GetUserSettings(currentUser);
        LoadSettings();
        LoadContacts();
    }

    private void LoadSettings()
    {
        SelectedThemeIndex = (int)_userSettings.Theme;
        WallpaperPath = _userSettings.WallpaperPath ?? "Нет обоев";
        
        InviteToChatsIndex = (int)_userSettings.InviteToChats;
        InviteToChannelsIndex = (int)_userSettings.InviteToChannels;
        PrivateMessagesIndex = (int)_userSettings.PrivateMessages;
        ShowAvatarIndex = (int)_userSettings.ShowAvatar;
        StreamerMode = _userSettings.StreamerMode;
    }

    private void LoadContacts()
    {
        Contacts.Clear();
        foreach (var contact in _contactService.GetUserContacts(_currentUser))
        {
            Contacts.Add(contact);
        }
    }

    private void SavePrivacySettings()
    {
        _userSettings.InviteToChats = (PrivacySetting)InviteToChatsIndex;
        _userSettings.InviteToChannels = (PrivacySetting)InviteToChannelsIndex;
        _userSettings.PrivateMessages = (PrivacySetting)PrivateMessagesIndex;
        _userSettings.ShowAvatar = (PrivacySetting)ShowAvatarIndex;
        _userSettings.StreamerMode = StreamerMode;
        _settingsService.UpdateUserSettings(_userSettings);
    }

    [RelayCommand]
    private void SaveProfile()
    {
        if (!string.IsNullOrWhiteSpace(NewNickname))
        {
            if (!_userService.IsNicknameValid(NewNickname))
            {
                ProfileErrorMessage = "Никнейм должен содержать только латиницу";
                ProfileHasError = true;
                return;
            }
        }
        
        if (!string.IsNullOrWhiteSpace(NewEmail) && NewEmail.Contains("@"))
        {
        }
        
        ProfileErrorMessage = "Данные сохранены!";
        ProfileHasError = false;
    }

    [RelayCommand]
    private void SelectAvatar()
    {
        AvatarPath = "new_avatar.png";
    }

    [RelayCommand]
    private void SaveTheme()
    {
        _userSettings.Theme = (AppTheme)SelectedThemeIndex;
        _settingsService.UpdateUserSettings(_userSettings);
    }

    [RelayCommand]
    private void SelectWallpaper()
    {
        WallpaperPath = "new_wallpaper.png";
        _userSettings.WallpaperPath = WallpaperPath;
        _settingsService.UpdateUserSettings(_userSettings);
    }

    [RelayCommand]
    private void SavePrivacy()
    {
        SavePrivacySettings();
    }

    [RelayCommand]
    private void UpdateContactName()
    {
        if (SelectedContact != null && !string.IsNullOrWhiteSpace(NewContactName))
        {
            _contactService.UpdateContactName(_currentUser, SelectedContact.ContactNickname, NewContactName);
            LoadContacts();
            NewContactName = string.Empty;
        }
    }

    [RelayCommand]
    private void CreatePrivateChat()
    {
    }

    [RelayCommand]
    private void Logout()
    {
        LogoutRequested?.Invoke();
    }

    [RelayCommand]
    private void ExitApp()
    {
        ExitAppRequested?.Invoke();
    }

    [RelayCommand]
    private void Close()
    {
        SettingsClosed?.Invoke();
    }

    [RelayCommand]
    private void ShowProfile() 
    { 
        SelectedTab = "Profile";
        OnPropertyChanged(nameof(SelectedTab));
    }
    
    [RelayCommand]
    private void ShowAppearance() 
    { 
        SelectedTab = "Appearance";
        OnPropertyChanged(nameof(SelectedTab));
    }
    
    [RelayCommand]
    private void ShowPrivacy() 
    { 
        SelectedTab = "Privacy";
        OnPropertyChanged(nameof(SelectedTab));
    }
    
    [RelayCommand]
    private void ShowContacts() 
    { 
        SelectedTab = "Contacts";
        OnPropertyChanged(nameof(SelectedTab));
    }
}