using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class CreateChannelViewModel : ObservableObject
{
    private readonly ChannelService _channelService;
    private readonly UserService _userService;
    private readonly string _currentUser;

    [ObservableProperty]
    private string _channelName = string.Empty;
    
    [ObservableProperty]
    private string _avatarPath = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;

    public event Action? ChannelCreated;
    public event Action? CancelRequested;

    public CreateChannelViewModel(ChannelService channelService, UserService userService, string currentUser)
    {
        _channelService = channelService;
        _userService = userService;
        _currentUser = currentUser;
    }

    [RelayCommand]
    private void SelectAvatar()
    {
        AvatarPath = "default_channel_avatar.png";
    }

    [RelayCommand]
    private void CreateChannel()
    {
        if (string.IsNullOrWhiteSpace(ChannelName))
        {
            ErrorMessage = "Введите название канала";
            HasError = true;
            return;
        }
        
        _channelService.CreateChannel(ChannelName, _currentUser, string.IsNullOrEmpty(AvatarPath) ? null : AvatarPath);
        ChannelCreated?.Invoke();
    }

    [RelayCommand]
    private void Cancel()
    {
        CancelRequested?.Invoke();
    }
}