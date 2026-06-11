using Avalonia.Controls;
using TelegromV4.ViewModels;

namespace TelegromV4.Views;

public partial class ChatsView : UserControl
{
    public ChatsView()
    {
        InitializeComponent();
    }
    
    public void ShowChatSettings(ChatSettingsViewModel settingsVm)
    {
        var settingsView = new ChatSettingsView();
        settingsView.DataContext = settingsVm;
        
        // Находим ContentControl в XAML
        var contentControl = this.FindControl<ContentControl>("SettingsContent");
        if (contentControl != null)
        {
            contentControl.Content = settingsView;
        }
    }
}