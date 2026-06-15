using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class UserProfileViewModel : ObservableObject
{
    private readonly UserService _userService;
    private readonly ContactService _contactService;
    private readonly PrivateChatService _privateChatService;
    private readonly string _currentUser;
    private readonly User _targetUser;

    [ObservableProperty]
    private string _nickname = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _avatarPath = string.Empty;
    
    [ObservableProperty]
    private bool _isContact;
    
    [ObservableProperty]
    private string _contactButtonText = string.Empty;

    public event Action? ProfileClosed;
    public event Action<string>? StartPrivateChatRequested;

    public UserProfileViewModel(UserService userService, ContactService contactService, PrivateChatService privateChatService, string currentUser, string targetNickname)
    {
        _userService = userService;
        _contactService = contactService;
        _privateChatService = privateChatService;
        _currentUser = currentUser;
        
        _targetUser = _userService.GetUserByNickname(targetNickname) ?? new User { Nickname = targetNickname, Email = "", AvatarPath = "" };
        
        Nickname = _targetUser.Nickname;
        Email = _targetUser.Email;
        AvatarPath = _targetUser.AvatarPath ?? "default_avatar.png";
        
        IsContact = _contactService.IsContact(currentUser, targetNickname);
        UpdateContactButtonText();
    }

    private void UpdateContactButtonText()
    {
        ContactButtonText = IsContact ? "❌ Удалить из контактов" : "➕ Добавить в контакты";
    }

    [RelayCommand]
    private void ToggleContact()
    {
        if (IsContact)
        {
            _contactService.RemoveContact(_currentUser, Nickname);
            IsContact = false;
        }
        else
        {
            _contactService.AddContact(_currentUser, Nickname);
            IsContact = true;
        }
        UpdateContactButtonText();
    }

    [RelayCommand]
    private void StartChat()
    {
        StartPrivateChatRequested?.Invoke(Nickname);
        ProfileClosed?.Invoke();
    }

    [RelayCommand]
    private void Close()
    {
        ProfileClosed?.Invoke();
    }
}