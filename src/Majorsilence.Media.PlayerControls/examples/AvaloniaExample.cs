// Majorsilence.Media.PlayerControls — Avalonia usage example.
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
//   dotnet add package Majorsilence.Forms.Avalonia
//
// The embedding snippet below (BuildPlayerHost/AttachToExistingPanel) was compiled against the
// real published packages while writing this example; the whole-app-on-Avalonia option is not
// duplicated here as code because it is exactly what src/MediaPlayer in this repo already is —
// see Player.cs, Player.Designer.cs and Program.cs there for the complete, working version.

using Majorsilence.Media.PlayerControls;
using Majorsilence.Forms;   // ToAvaloniaControl() ships in the Avalonia backend package, in this namespace
using Majorsilence.Media.Videos;

namespace Majorsilence.Media.PlayerControls.Examples.Avalonia;

// ── Embed the control inside an existing (XAML) Avalonia app ──────────────────────────────────
internal static class EmbedInExistingAvaloniaApp
{
    // Avalonia.Controls.Control and Majorsilence.Forms.Control share a name — both types are kept
    // fully qualified here (rather than `using Avalonia.Controls;` alongside `using
    // Majorsilence.Forms;`) so the two don't collide.
    private static global::Avalonia.Controls.Control BuildPlayerHost()
    {
        var player = new WinFormMPlayerControl
        {
            VideoPath = @"C:\videos\sample.mp4",
            MPlayerPath = @"C:\mpv\libmpv-2.dll",
        };
        player.SetPlayer(PlayerFactory.Get(player.Handle, player.MPlayerPath));

        // ToAvaloniaControl() installs the Avalonia backend automatically if none is configured
        // yet, and hands back a real Avalonia.Controls.Control you can drop into any visual tree.
        return player.ToAvaloniaControl();
    }

    private static void AttachToExistingPanel(global::Avalonia.Controls.Panel existingPanel)
    {
        existingPanel.Children.Add(BuildPlayerHost());
    }
}
