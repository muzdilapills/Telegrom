using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TelegromV4.Models;

namespace TelegromV4.Services;

public class SettingsService
{
    private const string SettingsFile = "userseitings_data.json";
    private const string ThemesFile = "userschatsthemes.json";
    private List<UserSettings> _settings = new List<UserSettings>();

    public SettingsService()
    {
        LoadSettings();
    }

    private void LoadSettings()
    {
        if (File.Exists(SettingsFile))
        {
            var json = File.ReadAllText(SettingsFile);
            _settings = JsonSerializer.Deserialize<List<UserSettings>>(json) ?? new List<UserSettings>();
        }
    }

    private void SaveSettings()
    {
        var json = JsonSerializer.Serialize(_settings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingsFile, json);
    }

    public UserSettings GetUserSettings(string nickname)
    {
        var settings = _settings.FirstOrDefault(s => s.Nickname == nickname);
        if (settings == null)
        {
            settings = new UserSettings { Nickname = nickname };
            _settings.Add(settings);
            SaveSettings();
        }
        return settings;
    }

    public void UpdateUserSettings(UserSettings settings)
    {
        var existing = _settings.FirstOrDefault(s => s.Nickname == settings.Nickname);
        if (existing != null)
        {
            existing.InviteToChats = settings.InviteToChats;
            existing.InviteToChannels = settings.InviteToChannels;
            existing.PrivateMessages = settings.PrivateMessages;
            existing.ShowAvatar = settings.ShowAvatar;
            existing.StreamerMode = settings.StreamerMode;
            existing.Theme = settings.Theme;
            existing.WallpaperPath = settings.WallpaperPath;
            existing.AvatarPath = settings.AvatarPath;
        }
        SaveSettings();
    }

    public void SaveTheme(string nickname, AppTheme theme, string? wallpaperPath)
    {
        var settings = GetUserSettings(nickname);
        settings.Theme = theme;
        settings.WallpaperPath = wallpaperPath;
        SaveSettings();
    }
}