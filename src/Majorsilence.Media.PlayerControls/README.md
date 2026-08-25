# Majorsilence.Media.PlayerControls

WinForms-style video playback controls — `WinFormMPlayerControl` (a ready-made panel with
play/stop buttons) and `VideoView` (the bare video surface, for building your own transport
controls) — for playing video through mplayer or libmpv from .NET.

The controls are built on [Majorsilence.Forms](https://www.nuget.org/packages/Majorsilence.Forms),
a WinForms-API-compatible UI toolkit that itself runs on more than one native windowing toolkit.
That means `WinFormMPlayerControl`/`VideoView` are **not** `System.Windows.Forms` controls — they
are `Majorsilence.Forms.Control`s — and the same control code runs unmodified on any of
Majorsilence.Forms' backends: **WinForms** (real `System.Windows.Forms` windows), **Avalonia**
(the default, cross-platform), and **Uno Platform**. Only how you *host* the control changes
between them; this document covers all three.

## Install

```bash
dotnet add package Majorsilence.Media.PlayerControls
dotnet add package Majorsilence.Media.Videos
```

Then add exactly one Majorsilence.Forms backend package for the platform you're targeting — see
the sections below.

## What you get

- **`WinFormMPlayerControl`** — a `UserControl` with a video panel plus Play/Stop buttons wired
  up already. Set `VideoPath`/`MPlayerPath` and call `SetPlayer`.
- **`VideoView`** — just the video-painting surface, no chrome. Use this when you're building your
  own transport controls (play/pause/seek/track bar) against `Majorsilence.Media.Videos.Player`
  directly — see [`src/Majorsilence.Media.Player/Player.cs`](../Majorsilence.Media.Player/Player.cs) in this repo for a
  complete example with a synced track bar, fast-forward/rewind, audio-track and subtitle
  selection, and fullscreen.

## Shared setup, every platform

However you host it, wiring a player to the control looks the same everywhere:

```csharp
using Majorsilence.Media.PlayerControls;
using Majorsilence.Media.Videos;

var player = new WinFormMPlayerControl
{
    VideoPath = @"C:\videos\sample.mp4",
    MPlayerPath = @"C:\mpv\libmpv-2.dll",   // or a path containing "mplayer" to use the mplayer backend instead
};

player.SetPlayer(PlayerFactory.Get(player.Handle, player.MPlayerPath));
```

`player.Handle` is always `0`. Majorsilence.Forms composites every control into one drawn
surface, so a control has no native window of its own to hand mpv/mplayer for embedding —
`VideoView` (already attached inside `WinFormMPlayerControl`) instead paints the frames the
player renders into memory, so passing the zero handle through is what tells the backend to do
that rather than open a separate native window. See [`VideoView.cs`](VideoView.cs).

Once `SetPlayer` has been called, either use the control's own Play/Stop buttons, or drive
`Player` yourself (`Play`, `Pause`, `Stop`, `Seek`, the `CurrentPosition`/`VideoExited` events,
...) the way `Player.cs` does.

## Two ways to host it

**A — Majorsilence.Forms owns the whole app (simplest).** Write the app with the familiar
WinForms-style Majorsilence.Forms API (`Form`, `Panel`, `Button`, `TrackBar`, ...) and drop
`WinFormMPlayerControl` in like any other control — exactly what
[`src/Majorsilence.Media.Player`](../Majorsilence.Media.Player) does. Only `Program.cs` (which backend package you reference,
and one line selecting it) differs between WinForms/Avalonia/Uno.

**B — Embed inside an app you already have.** If you have an existing native WinForms, Avalonia,
or Uno app and just want the player inside one panel or window, wrap the control with
`ToWinFormsControl()` / `ToAvaloniaControl()` / `ToUnoControl()`.

Both are shown per-platform below, and as complete files in [`examples/`](examples).

---

## WinForms

### Whole app on WinForms

```bash
dotnet add package Majorsilence.Forms
dotnet add package Majorsilence.Forms.WinForms
```

> **`Majorsilence.Forms.WinForms` is Windows-only and, as of this writing, not yet published to
> nuget.org** — build it from the `src/Majorsilence.Forms.WinForms` project in
> [majorsilence/Majorsilence.Forms](https://github.com/majorsilence/Majorsilence.Forms) until it
> ships, or use the Avalonia backend below in the meantime. The control code is identical either
> way — only this selection line changes.

```csharp
[STAThread]
private static void Main()
{
    Majorsilence.Forms.Backends.Platform.Backend = new Majorsilence.Forms.WinForms.WinFormsPlatformBackend();
    Majorsilence.Forms.Application.Run(new MainForm());
}
```

`MainForm` is an ordinary `Majorsilence.Forms.Form` containing a `WinFormMPlayerControl` —
[`src/Majorsilence.Media.Player/Player.cs`](../Majorsilence.Media.Player/Player.cs) in this repo is a complete one (it
currently runs on the Avalonia backend by default; the `Backend = ...` line above is the only
change needed to run that same form on real WinForms windows instead).

### Embed inside an existing System.Windows.Forms app

```bash
dotnet add package Majorsilence.Forms.WinForms
```

```csharp
using Majorsilence.Forms.WinForms;

var player = new Majorsilence.Media.PlayerControls.WinFormMPlayerControl { VideoPath = "...", MPlayerPath = "..." };
player.SetPlayer(Majorsilence.Media.Videos.PlayerFactory.Get(player.Handle, player.MPlayerPath));

System.Windows.Forms.Control host = player.ToWinFormsControl();   // a real WinForms Control
host.Dock = System.Windows.Forms.DockStyle.Fill;
myExistingWinFormsForm.Controls.Add(host);
```

Full example: [`examples/WinFormsExample.cs`](examples/WinFormsExample.cs).

---

## Avalonia

Avalonia is the default backend — reference it and there is nothing else to configure:

```bash
dotnet add package Majorsilence.Forms
dotnet add package Majorsilence.Forms.Avalonia
```

### Whole app on Avalonia

This is exactly what [`src/Majorsilence.Media.Player`](../Majorsilence.Media.Player) in this repo already does — see
`Player.cs`/`Player.Designer.cs`/`Program.cs` for the complete player.

### Embed inside an existing (XAML) Avalonia app

```csharp
using Majorsilence.Forms;   // ToAvaloniaControl() ships in the Avalonia backend package, in this namespace

var player = new Majorsilence.Media.PlayerControls.WinFormMPlayerControl { VideoPath = "...", MPlayerPath = "..." };
player.SetPlayer(Majorsilence.Media.Videos.PlayerFactory.Get(player.Handle, player.MPlayerPath));

Avalonia.Controls.Control hostControl = player.ToAvaloniaControl();
myExistingAvaloniaPanel.Children.Add(hostControl);
```

`Avalonia.Controls.Control` and `Majorsilence.Forms.Control` share a name — keep them qualified as
above (or alias one) rather than `using Avalonia.Controls;` and `using Majorsilence.Forms;`
together unqualified, or the compiler can't tell them apart.

Full example: [`examples/AvaloniaExample.cs`](examples/AvaloniaExample.cs).
[`src/Majorsilence.Media.Desktop.UI`](../Majorsilence.Media.Desktop.UI) in this repo is a native
Avalonia app; it currently paints video with its own hand-rolled control rather than hosting
`WinFormMPlayerControl` this way, but it's the kind of app this embedding pattern is for.

---

## Uno Platform

```bash
dotnet add package Majorsilence.Forms
dotnet add package Majorsilence.Forms.Uno
```

Unlike Avalonia, this backend runs inside an **Uno app head** — an Uno project that boots the Uno
platform and installs the backend on launch, rather than a `Program.cs` that just calls
`Application.Run`.

### Whole app on Uno

```csharp
using Microsoft.UI.Xaml;
using Majorsilence.Forms;
using Majorsilence.Forms.Uno;
using Uno.UI.Hosting;

public static class Program
{
    [STAThread]
    public static void Main()
    {
        var host = UnoPlatformHostBuilder.Create()
            .App(() => new MediaPlayerUnoApp())
            .UseX11().UseWin32().UseMacOS()
            .Build();

        host.Run();
    }
}

public sealed class MediaPlayerUnoApp : Application
{
    protected override void OnLaunched(LaunchActivatedEventArgs args)
    {
        Majorsilence.Forms.Backends.Platform.Backend = new UnoPlatformBackend();
        new MainForm().Show();   // MainForm: a Majorsilence.Forms.Form containing WinFormMPlayerControl
    }
}
```

Start from [`Gallery.Uno`](https://github.com/majorsilence/Majorsilence.Forms/tree/main/samples/Gallery.Uno)
in the Majorsilence.Forms repo for a complete, working app head rather than wiring one by hand.

### Embed inside an existing Uno app

```csharp
using Majorsilence.Forms.Uno;

var player = new Majorsilence.Media.PlayerControls.WinFormMPlayerControl { VideoPath = "...", MPlayerPath = "..." };
player.SetPlayer(Majorsilence.Media.Videos.PlayerFactory.Get(player.Handle, player.MPlayerPath));

Microsoft.UI.Xaml.FrameworkElement hostControl = player.ToUnoControl();
MyExistingGrid.Children.Add(hostControl);
```

Full example: [`examples/UnoExample.cs`](examples/UnoExample.cs).

---

## mplayer / libmpv setup

`Majorsilence.Media.Videos` (which `PlayerFactory` comes from) needs either `mplayer` or libmpv
available — see the [top-level README](../../Readme.md#mplayer) for install/path/`LC_NUMERIC`
details for each platform.

## Links

- [Platform backends](https://github.com/majorsilence/Majorsilence.Forms/blob/main/docs/backends.md) —
  the full embedding API (`ToXControl`/`ToXWindow`) and backend concepts this document builds on
- [Majorsilence.Forms documentation](https://forms.majorsilence.com)
- [Repository](https://github.com/majorsilence/MPlayerControl)

Licensed under the LGPL v2.1 or later — see [`lgpl-2.1.txt`](../../lgpl-2.1.txt).
