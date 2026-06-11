using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class ChatSettingsViewModel : ObservableObject
{
    private readonly ChatService _chatService;
    private readonly UserService _userService;
    private readonly Chat _chat;
    private readonly string _currentUser;
    private readonly bool _isAdmin;

    [ObservableProperty]
    private string _chatName = string.Empty;
    
    [ObservableProperty]
    private string _newChatName = string.Empty;
    
    [ObservableProperty]
    private ObservableCollection<ChatMember> _members = new ObservableCollection<ChatMember>();
    
    [ObservableProperty]
    private ChatMember? _selectedMember;
    
    [ObservableProperty]
    private ObservableCollection<User> _availableUsers = new ObservableCollection<User>();
    
    [ObservableProperty]
    private User? _selectedUserToAdd;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;

    public bool CanManage => _isAdmin;
    public string CurrentUserRole => GetUserRole();

    public event Action? SettingsClosed;
    public event Action? ChatUpdated;

    public ChatSettingsViewModel(ChatService chatService, UserService userService, Chat chat, string currentUser)
    {
        _chatService = chatService;
        _userService = userService;
        _chat = chat;
        _currentUser = currentUser;
        
        var currentMember = chat.Members.FirstOrDefault(m => m.Nickname == currentUser);
        _isAdmin = currentMember != null && (currentMember.Role == ChatRole.Admin || currentMember.Role == ChatRole.Creator);
        
        LoadData();
    }

    private void LoadData()
    {
        ChatName = _chat.Name;
        NewChatName = _chat.Name;
        
        Members.Clear();
        foreach (var member in _chat.Members)
        {
            Members.Add(member);
        }
        
        LoadAvailableUsers();
    }

    private void LoadAvailableUsers()
    {
        AvailableUsers.Clear();
        var users = _chatService.GetAvailableUsersToAdd(_chat.Id, _userService.GetAllUsers());
        foreach (var user in users)
        {
            AvailableUsers.Add(user);
        }
    }

    private string GetUserRole()
    {
        var member = _chat.Members.FirstOrDefault(m => m.Nickname == _currentUser);
        if (member == null) return "Не участник";
        return member.Role switch
        {
            ChatRole.Creator => "Создатель",
            ChatRole.Admin => "Администратор",
            _ => "Участник"
        };
    }

    [RelayCommand]
    private void ChangeName()
    {
        if (!_isAdmin)
        {
            ErrorMessage = "Только администраторы могут менять название";
            HasError = true;
            return;
        }
        
        if (string.IsNullOrWhiteSpace(NewChatName))
        {
            ErrorMessage = "Введите название чата";
            HasError = true;
            return;
        }
        
        if (_chatService.UpdateChatName(_chat.Id, _currentUser, NewChatName))
        {
            ChatName = NewChatName;
            ChatUpdated?.Invoke();
            ErrorMessage = "Название изменено!";
            HasError = false;
        }
    }

    [RelayCommand]
    private void AddMember()
    {
        if (!_isAdmin)
        {
            ErrorMessage = "Только администраторы могут добавлять участников";
            HasError = true;
            return;
        }
        
        if (SelectedUserToAdd == null)
        {
            ErrorMessage = "Выберите пользователя";
            HasError = true;
            return;
        }
        
        if (_chatService.AddMemberToChat(_chat.Id, _currentUser, SelectedUserToAdd.Nickname))
        {
            LoadData();
            ChatUpdated?.Invoke();
            ErrorMessage = "Участник добавлен!";
            HasError = false;
        }
    }

    [RelayCommand]
    private void RemoveMember()
    {
        if (!_isAdmin)
        {
            ErrorMessage = "Только администраторы могут удалять участников";
            HasError = true;
            return;
        }
        
        if (SelectedMember == null)
        {
            ErrorMessage = "Выберите участника";
            HasError = true;
            return;
        }
        
        if (SelectedMember.Role == ChatRole.Creator)
        {
            ErrorMessage = "Нельзя удалить создателя чата";
            HasError = true;
            return;
        }
        
        if (_chatService.RemoveMemberFromChat(_chat.Id, _currentUser, SelectedMember.Nickname))
        {
            LoadData();
            ChatUpdated?.Invoke();
            ErrorMessage = "Участник удалён!";
            HasError = false;
        }
    }

    [RelayCommand]
    private void MakeAdmin()
    {
        var creator = _chat.Members.FirstOrDefault(m => m.Nickname == _currentUser);
        if (creator?.Role != ChatRole.Creator)
        {
            ErrorMessage = "Только создатель чата может назначать администраторов";
            HasError = true;
            return;
        }
        
        if (SelectedMember == null)
        {
            ErrorMessage = "Выберите участника";
            HasError = true;
            return;
        }
        
        if (SelectedMember.Role == ChatRole.Creator)
        {
            ErrorMessage = "Создатель всегда администратор";
            HasError = true;
            return;
        }
        
        if (_chatService.MakeAdmin(_chat.Id, _currentUser, SelectedMember.Nickname))
        {
            LoadData();
            ChatUpdated?.Invoke();
            ErrorMessage = $"{SelectedMember.Nickname} назначен администратором!";
            HasError = false;
        }
    }

    [RelayCommand]
    private void RemoveAdmin()
    {
        var creator = _chat.Members.FirstOrDefault(m => m.Nickname == _currentUser);
        if (creator?.Role != ChatRole.Creator)
        {
            ErrorMessage = "Только создатель чата может снимать администраторов";
            HasError = true;
            return;
        }
        
        if (SelectedMember == null)
        {
            ErrorMessage = "Выберите администратора";
            HasError = true;
            return;
        }
        
        if (SelectedMember.Role != ChatRole.Admin)
        {
            ErrorMessage = "Выбранный пользователь не является администратором";
            HasError = true;
            return;
        }
        
        if (_chatService.RemoveAdmin(_chat.Id, _currentUser, SelectedMember.Nickname))
        {
            LoadData();
            ChatUpdated?.Invoke();
            ErrorMessage = $"С {SelectedMember.Nickname} снята роль администратора!";
            HasError = false;
        }
    }

    [RelayCommand]
    private void DeleteChat()
    {
        var creator = _chat.Members.FirstOrDefault(m => m.Nickname == _currentUser);
        if (creator?.Role != ChatRole.Creator)
        {
            ErrorMessage = "Только создатель чата может удалить его";
            HasError = true;
            return;
        }
        
        if (_chatService.DeleteChat(_chat.Id, _currentUser))
        {
            ChatUpdated?.Invoke();
            SettingsClosed?.Invoke();
        }
    }

    [RelayCommand]
    private void LeaveChat()
    {
        if (_chat.CreatorNickname == _currentUser)
        {
            ErrorMessage = "Создатель не может покинуть чат. Используйте 'Удалить чат'";
            HasError = true;
            return;
        }
        
        if (_chatService.LeaveChat(_chat.Id, _currentUser))
        {
            SettingsClosed?.Invoke();
        }
    }

    [RelayCommand]
    private void Close()
    {
        SettingsClosed?.Invoke();
    }
}