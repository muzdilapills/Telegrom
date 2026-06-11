using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class CreateChatViewModel : ObservableObject
{
    private readonly ChatService _chatService;
    private readonly string _currentUser;

    [ObservableProperty]
    private string _chatName = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;

    public event Action? ChatCreated;
    public event Action? CancelRequested;

    public CreateChatViewModel(ChatService chatService, string currentUser)
    {
        _chatService = chatService;
        _currentUser = currentUser;
    }

    [RelayCommand]
    private void CreateChat()
    {
        if (string.IsNullOrWhiteSpace(ChatName))
        {
            ErrorMessage = "Введите название чата";
            HasError = true;
            return;
        }
        
        _chatService.CreateChat(ChatName, _currentUser);
        ChatCreated?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelRequested?.Invoke();
    }
}