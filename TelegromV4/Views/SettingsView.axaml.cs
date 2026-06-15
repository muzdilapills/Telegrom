using Avalonia.Controls;
using TelegromV4.ViewModels;

namespace TelegromV4.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
        
        // Подписываемся на события ViewModel
        DataContextChanged += OnDataContextChanged;
    }
    
    private void OnDataContextChanged(object? sender, System.EventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
        {
            vm.PropertyChanged += (s, args) =>
            {
                if (args.PropertyName == nameof(SettingsViewModel.SelectedTab))
                {
                    SwitchTab(vm.SelectedTab);
                }
            };
            // Инициализация
            SwitchTab(vm.SelectedTab);
        }
    }
    
    private void SwitchTab(string tab)
    {
        // Находим все панели
        var profilePanel = this.FindControl<StackPanel>("ProfilePanel");
        var appearancePanel = this.FindControl<StackPanel>("AppearancePanel");
        var privacyPanel = this.FindControl<StackPanel>("PrivacyPanel");
        var contactsPanel = this.FindControl<StackPanel>("ContactsPanel");
        
        // Скрываем все
        if (profilePanel != null) profilePanel.IsVisible = false;
        if (appearancePanel != null) appearancePanel.IsVisible = false;
        if (privacyPanel != null) privacyPanel.IsVisible = false;
        if (contactsPanel != null) contactsPanel.IsVisible = false;
        
        // Показываем выбранную
        switch (tab)
        {
            case "Profile":
                if (profilePanel != null) profilePanel.IsVisible = true;
                break;
            case "Appearance":
                if (appearancePanel != null) appearancePanel.IsVisible = true;
                break;
            case "Privacy":
                if (privacyPanel != null) privacyPanel.IsVisible = true;
                break;
            case "Contacts":
                if (contactsPanel != null) contactsPanel.IsVisible = true;
                break;
        }
    }
}