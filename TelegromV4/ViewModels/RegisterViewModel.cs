using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class RegisterViewModel : ObservableObject
{
    private readonly UserService _userService;
    
    [ObservableProperty]
    private string _nickname = string.Empty;
    
    [ObservableProperty]
    private string _email = string.Empty;
    
    [ObservableProperty]
    private string _password = string.Empty;
    
    [ObservableProperty]
    private string _confirmPassword = string.Empty;
    
    [ObservableProperty]
    private string _errorMessage = string.Empty;
    
    [ObservableProperty]
    private bool _hasError;

    public event Action? RegisterSuccess;
    public event Action? LoginRequested;

    public RegisterViewModel(UserService userService)
    {
        _userService = userService;
    }

    [RelayCommand]
    private void Register()
    {
        if (string.IsNullOrWhiteSpace(Nickname))
        {
            ErrorMessage = "Введите никнейм";
            HasError = true;
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
        {
            ErrorMessage = "Введите корректный email";
            HasError = true;
            return;
        }
        
        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 4)
        {
            ErrorMessage = "Пароль должен быть не менее 4 символов";
            HasError = true;
            return;
        }
        
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Пароли не совпадают";
            HasError = true;
            return;
        }
        
        var success = _userService.Register(Nickname, Email, Password);
        
        if (success)
        {
            HasError = false;
            RegisterSuccess?.Invoke();
        }
        else
        {
            ErrorMessage = "Пользователь с таким никнеймом уже существует";
            HasError = true;
        }
    }

    [RelayCommand]
    private void GoToLogin()
    {
        LoginRequested?.Invoke();
    }
}