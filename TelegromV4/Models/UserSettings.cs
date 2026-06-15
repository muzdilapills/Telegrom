namespace TelegromV4.Models;

public class UserSettings
{
    public string Nickname { get; set; } = string.Empty;
    
    // Настройки приватности
    public PrivacySetting InviteToChats { get; set; } = PrivacySetting.Everyone;
    public PrivacySetting InviteToChannels { get; set; } = PrivacySetting.Everyone;
    public PrivacySetting PrivateMessages { get; set; } = PrivacySetting.Everyone;
    public PrivacySetting ShowAvatar { get; set; } = PrivacySetting.Everyone;
    public bool StreamerMode { get; set; } = false;
    
    // Настройки оформления
    public AppTheme Theme { get; set; } = AppTheme.Dark;
    public string? WallpaperPath { get; set; }
    
    // Путь к аватарке
    public string? AvatarPath { get; set; }
}

public enum PrivacySetting
{
    Everyone,
    ContactsOnly,
    Nobody
}

public enum AppTheme
{
    Light,
    DarkLight,
    Dark,
    DarkRed
}