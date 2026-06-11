using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class LoginViewModel : ObservableObject
{
    private readonly UserService _userService;
    private readonly AdminService _adminService;
    
    [ObservableProperty]
    private string _nickname = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;

    public event Action? RegisterRequested;
    public event Action<string>? LoginSuccess;

    public LoginViewModel(UserService userService, AdminService adminService)
    {
        _userService = userService;
        _adminService = adminService;
    }

    [RelayCommand]
    private void Login()
    {
        if (string.IsNullOrWhiteSpace(Nickname) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Заполните все поля";
            HasError = true;
            return;
        }

        var user = _userService.Login(Nickname, Password);
        
        if (user != null)
        {
            HasError = false;
            LoginSuccess?.Invoke(Nickname);
        }
        else
        {
            ErrorMessage = "Неверный никнейм или пароль";
            HasError = true;
        }
    }

    [RelayCommand]
    private void GoToRegister()
    {
        RegisterRequested?.Invoke();
    }
}