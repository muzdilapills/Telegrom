using Avalonia.Controls;
using Avalonia.Input;
using TelegromV4.ViewModels;

namespace TelegromV4.Views;

public partial class ChatsView : UserControl
{
    public ChatsView()
    {
        InitializeComponent();
        KeyDown += OnKeyDown;
    }
    
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Escape && DataContext is ChatsViewModel vm)
        {
            vm.SelectedChatItem = null;
        }
    }
    
    public void ShowChatSettings(ChatSettingsViewModel settingsVm)
    {
        var settingsView = new ChatSettingsView();
        settingsView.DataContext = settingsVm;
        
        var contentControl = this.FindControl<ContentControl>("SettingsContent");
        if (contentControl != null)
        {
            contentControl.Content = settingsView;
        }
    }
}