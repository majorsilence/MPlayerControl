using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;

namespace Majorsilence.Media.Desktop.UI;

public partial class PlayerProperties : Window
{
    public PlayerProperties()
    {
        InitializeComponent();
    }

    private async void OnBrowseButtonClick(object? sender, RoutedEventArgs e)
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions());

        if (files.Count > 0)
        {
            var localPath = files[0].TryGetLocalPath();
            if (!string.IsNullOrWhiteSpace(localPath))
            {
                PlayerPathTextBox.Text = localPath;
            }
        }
    }

    private void OnSaveButtonClick(object? sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.MPlayerPath = PlayerPathTextBox.Text.Trim();
        Properties.Settings.Default.Save();
    }
}
