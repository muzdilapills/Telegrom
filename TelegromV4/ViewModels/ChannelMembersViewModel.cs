using System;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using TelegromV4.Models;
using TelegromV4.Services;

namespace TelegromV4.ViewModels;

public partial class ChannelMembersViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ChannelMemberDisplay> _members = new ObservableCollection<ChannelMemberDisplay>();

    public event Action? Closed;

    public ChannelMembersViewModel(Channel channel)
    {
        foreach (var member in channel.Members)
        {
            string roleText = member.Role switch
            {
                ChannelRole.Creator => "Создатель",
                ChannelRole.Admin => "Админ",
                _ => "Подписчик"
            };
            Members.Add(new ChannelMemberDisplay { Nickname = member.Nickname, Role = roleText });
        }
    }

    [RelayCommand]
    private void Close()
    {
        Closed?.Invoke();
    }
}

public class ChannelMemberDisplay
{
    public string Nickname { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}