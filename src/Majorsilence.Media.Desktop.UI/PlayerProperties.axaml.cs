using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Majorsilence.Media.Desktop.UI;

public partial class PlayerProperties : Window
{
    public PlayerProperties()
    {
        InitializeComponent();
    }

    private async void OnBrowseButtonClick(object? sender, RoutedEventArgs e)
    {
        var openFileDialog = new OpenFileDialog
        {
            AllowMultiple = false
        };

        var result = await openFileDialog.ShowAsync(this);
        if (result != null && result.Length > 0)
        {
            PlayerPathTextBox.Text = result[0];
        }
    }

    private void OnSaveButtonClick(object? sender, RoutedEventArgs e)
    {
        Properties.Settings.Default.MPlayerPath = PlayerPathTextBox.Text.Trim();
        Properties.Settings.Default.Save();
    }
}
