// LibMPlayerWinform — WinForms usage example.
//
// This file is documentation, not a buildable project: it isn't compiled into the
// LibMPlayerWinform assembly (see the `Compile Remove` in LibMPlayerWinform.csproj) and isn't
// meant to be dropped into your project as-is. Copy the parts you need. See ../README.md for
// the full write-up.
//
// Packages needed:
//   dotnet add package LibMPlayerWinform
//   dotnet add package Majorsilence.Media.Videos
//   dotnet add package Majorsilence.Forms
//   dotnet add package Majorsilence.Forms.WinForms
//
// Majorsilence.Forms.WinForms is Windows-only and, as of this writing, not yet published to
// nuget.org — build it from the Majorsilence.Forms repo's src/Majorsilence.Forms.WinForms
// project until it ships, or use the Avalonia example in the meantime; the control code below
// is identical either way.

using System;
using LibMPlayerWinform;
using Majorsilence.Forms;
using Majorsilence.Forms.WinForms;
using Majorsilence.Media.Videos;

namespace LibMPlayerWinform.Examples.WinForms;

// ── Option A: Majorsilence.Forms owns the whole app, running on real WinForms windows ─────────
internal static class WholeAppOnWinForms
{
    [STAThread]
    private static void Main()
    {
        // This is the only line that differs from running the same MainForm on Avalonia or Uno.
        Backends.Platform.Backend = new WinFormsPlatformBackend();

        Application.Run(new MainForm());
    }
}

// A Majorsilence.Forms.Form (WinForms-style API), not a System.Windows.Forms.Form. Compare with
// the complete, working version of this pattern at src/MediaPlayer/Player.cs in this repo, which
// adds play/pause/seek, a synced track bar, audio/subtitle track selection, and fullscreen.
internal sealed class MainForm : Form
{
    public MainForm()
    {
        Text = "LibMPlayerWinform on WinForms";
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

// ── Option B: embed the control inside an existing System.Windows.Forms app ────────────────────
internal static class EmbedInExistingWinFormsApp
{
    private static System.Windows.Forms.Control BuildPlayerHost()
    {
        var player = new WinFormMPlayerControl
        {
            VideoPath = @"C:\videos\sample.mp4",
            MPlayerPath = @"C:\mpv\libmpv-2.dll",
        };
        player.SetPlayer(PlayerFactory.Get(player.Handle, player.MPlayerPath));

        // ToWinFormsControl() installs the WinForms backend automatically if none is configured
        // yet, and hands back a real System.Windows.Forms.Control you can drop anywhere.
        System.Windows.Forms.Control host = player.ToWinFormsControl();
        host.Dock = System.Windows.Forms.DockStyle.Fill;
        return host;
    }

    private static void AttachToShellForm(System.Windows.Forms.Form shell)
    {
        shell.Controls.Add(BuildPlayerHost());
    }
}
