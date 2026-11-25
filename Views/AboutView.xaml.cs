using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using iNKORE.UI.WPF.Modern.Controls;
using Microsoft.Extensions.DependencyInjection;
using SteamLuaManager;
using SteamLuaManager.Services;

namespace SteamLuaManager.Views;

public partial class AboutView : UserControl, INotifyPropertyChanged
{
    private const string ProjectUrl = "https://github.com/lgtlgt0015-code";

    private readonly string? _version;
    public string VersionText => L.Get("About_Version", _version ?? "1.0.0");

    public event PropertyChangedEventHandler? PropertyChanged;

    public AboutView()
    {
        _version = Assembly.GetExecutingAssembly().GetName().Version?.ToString();
        InitializeComponent();
        DataContext = this;
        // View 常驻缓存：语言切换时重算版本行文案，其余动态弹窗在打开时按当前语言生成
        LocalizationService.Instance.PropertyChanged += (_, _) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(VersionText)));
    }

    private void GitHub_Click(object sender, RoutedEventArgs e)
    {
        Process.Start(new ProcessStartInfo(ProjectUrl) { UseShellExecute = true });
    }

    private async void CheckUpdate_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var updateService = App.ServiceProvider?.GetRequiredService<IUpdateService>();
            if (updateService == null) return;
            var result = await updateService.CheckForUpdateAsync();

            if (!result.HasPublishedRelease)
                await ShowDialogAsync(L.Get("About_CheckUpdate"), L.Get("About_UpdNoRelease"));
            else if (result.HasUpdate)
                await App.ShowUpdateLogDialogAsync(result);
            else
                await ShowDialogAsync(L.Get("About_UpdLatestTitle"), L.Get("About_UpdLatestBody", result.CurrentVersion));
        }
        catch (Exception ex)
        {
            await ShowDialogAsync(L.Get("About_UpdFailTitle"), L.Get("About_UpdFailBody", ex.Message));
        }
    }

    private static async Task ShowDialogAsync(string title, string message)
    {
        var dialog = new ContentDialog
        {
            Title = title,
            Content = new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap,
                MaxWidth = 420
            },
            CloseButtonText = L.Get("Common_Confirm"),
            DefaultButton = ContentDialogButton.Close
        };
        await dialog.ShowAsync();
    }
}
