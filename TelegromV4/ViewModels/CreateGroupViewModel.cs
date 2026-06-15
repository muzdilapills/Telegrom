using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class CreateGroupViewModel : ObservableObject
{
    private readonly ChatService _chatService;
    private readonly UserService _userService;
    private readonly string _currentUser;

    [ObservableProperty]
    private string _groupName = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<User> _availableUsers = new ObservableCollection<User>();
    
    [ObservableProperty]
    private ObservableCollection<User> _selectedUsers = new ObservableCollection<User>();
    
    [ObservableProperty]
    private User? _selectedUser;
    
    [ObservableProperty]
    private string _avatarPath = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;
    
    [ObservableProperty]
    private bool _isUserSelected;

    public event Action? GroupCreated;
    public event Action? CancelRequested;

    public CreateGroupViewModel(ChatService chatService, UserService userService, string currentUser)
    {
        _chatService = chatService;
        _userService = userService;
        _currentUser = currentUser;
        
        LoadUsers();
    }

    private void LoadUsers()
    {
        AvailableUsers.Clear();
        // Исключаем текущего пользователя
        foreach (var user in _userService.GetAllUsers().Where(u => u.Nickname != _currentUser))
        {
            AvailableUsers.Add(user);
        }
    }

    partial void OnSelectedUserChanged(User? value)
    {
        IsUserSelected = value != null;
    }

    [RelayCommand]
    private void AddUser()
    {
        if (SelectedUser != null && !SelectedUsers.Contains(SelectedUser))
        {
            SelectedUsers.Add(SelectedUser);
            AvailableUsers.Remove(SelectedUser);
            SelectedUser = null;
        }
    }

    [RelayCommand]
    private void RemoveUser(User user)
    {
        if (SelectedUsers.Contains(user))
        {
            SelectedUsers.Remove(user);
            AvailableUsers.Add(user);
        }
    }

    [RelayCommand]
    private void SelectAvatar()
    {
        // Временно пропускаем, позже добавим диалог выбора файла
        AvatarPath = "default_avatar.png";
    }

    [RelayCommand]
    private void CreateGroup()
    {
        if (string.IsNullOrWhiteSpace(GroupName))
        {
            ErrorMessage = "Введите название группы";
            HasError = true;
            return;
        }
        
        var chat = _chatService.CreateChat(GroupName, _currentUser);
        
        // Добавляем выбранных участников
        foreach (var user in SelectedUsers)
        {
            _chatService.AddMemberToChat(chat.Id, _currentUser, user.Nickname);
        }
        
        GroupCreated?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelRequested?.Invoke();
    }
}