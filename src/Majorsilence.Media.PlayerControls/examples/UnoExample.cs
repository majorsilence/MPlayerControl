// Majorsilence.Media.PlayerControls — Uno Platform usage example.
//
// This file is documentation, not a buildable project: it isn't compiled into the
// Majorsilence.Media.PlayerControls assembly (see the `Compile Remove` in Majorsilence.Media.PlayerControls.csproj) and isn't
// meant to be dropped into your project as-is. Copy the parts you need. See ../README.md for
// the full write-up.
//
// Packages needed:
//   dotnet add package Majorsilence.Media.PlayerControls
//   dotnet add package Majorsilence.Media.Videos
//   dotnet add package Majorsilence.Forms
//   dotnet add package Majorsilence.Forms.Uno
//
// Both patterns below were compiled against the real published packages while writing this
// example. Running the whole-app option for real additionally needs an Uno app head project
// (platform runtime packages, app manifest, etc.) — start from the Majorsilence.Forms repo's
// samples/Gallery.Uno rather than wiring one by hand; WholeAppOnUno below mirrors its Program.cs.

using System;
using Majorsilence.Media.PlayerControls;
using Majorsilence.Forms;
using Majorsilence.Forms.Uno;
using Majorsilence.Media.Videos;
using Microsoft.UI.Xaml;
using Uno.UI.Hosting;

namespace Majorsilence.Media.PlayerControls.Examples.Uno;

// ── Option A: Majorsilence.Forms owns the whole app, running on an Uno app head ────────────────
internal static class WholeAppOnUno
{
    [STAThread]
    private static void Main()
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new MediaPlayerUnoApp())
            .UseX11().UseWin32().UseMacOS()
            .Build();

        host.Run();
    }
}

// The Uno application: once launched, install the Uno backend and show a Majorsilence.Forms window.
internal sealed class MediaPlayerUnoApp : Application
{
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Backends.Platform.Backend = new UnoPlatformBackend();
        new MainForm().Show();
    }
}

// A Majorsilence.Forms.Form (WinForms-style API), not a WinUI window. Compare with the complete,
// working version of this pattern at src/MediaPlayer/Player.cs in this repo, which adds
// play/pause/seek, a synced track bar, audio/subtitle track selection, and fullscreen.
internal sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "Majorsilence.Media.PlayerControls on Uno";
        Width = 900;
        Height = 600;

        var player = new WinFormMPlayerControl
        {
            Dock = DockStyle.Fill,
            VideoPath = @"C:\videos\sample.mp4",
            MPlayerPath = @"C:\mpv\libmpv-2.dll",
        };
        player.SetPlayer(PlayerFactory.Get(player.Handle, player.MPlayerPath));

        Controls.Add(player);
    }
}

// ── Option B: embed the control inside an existing Uno app ─────────────────────────────────────
internal static class EmbedInExistingUnoApp
{
    private static Microsoft.UI.Xaml.FrameworkElement BuildPlayerHost()
    {
        var player = new WinFormMPlayerControl
        {
            VideoPath = @"C:\videos\sample.mp4",
            MPlayerPath = @"C:\mpv\libmpv-2.dll",
        };
        player.SetPlayer(PlayerFactory.Get(player.Handle, player.MPlayerPath));

        // ToUnoControl() installs the Uno backend automatically if none is configured yet, and
        // hands back a real WinUI FrameworkElement you can drop into any XAML tree.
        return player.ToUnoControl();
    }

    private static void AttachToExistingGrid(Microsoft.UI.Xaml.Controls.Panel existingGrid)
    {
        existingGrid.Children.Add(BuildPlayerHost());
    }
}
