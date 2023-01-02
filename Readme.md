This is a net6.0 wrapper library for MPlayer, libmpv, mencoder, and as of 1.8+ ffmpeg, and ffprobe support has been added.

There is a sample net6.0 winforms project included and it can play both audio and video files.

The sample player currently supports play, pause, stop. seek and some other basic functionality. I only add new features as I require them or people send in patches.

See examples on the wiki pages https://github.com/majorsilence/MPlayerControl/wiki.

This is a personal project that as of 2022 is just for me to play around with things.  If you are a dotnet developer looking for a way to interact with video and audio I suggest one of these projects instead.

* https://github.com/mpv-player/mpv-examples/tree/master/libmpv/csharp
* https://github.com/videolan/libvlcsharp


![MediaPlayer screeenshot](https://raw.githubusercontent.com/majorsilence/MPlayerControl/main/MediaPlayer.webp)

# Downloads

## Version 1.8+, net6.0

It has been tested on Windows 11 and Ubuntu Linux 22.04.  Use libmpv, ffmpeg, and ffprobe.

Mainly namespace changes, removal of old code, and the addition of ffmpeg and ffprobe. For example

* using LibMPlayerCommon; 
    * changes to:
    * using Majorsilence.Media.Videos;

TODO:

## Version 1.6 or less, net48

It has been tested on Windows XP/7/8 and Ubuntu Linux 10.10/11.04/12.04/13.04/14.04/16.04 and OpenSuse Linux 10.3.

Download using nuget https://www.nuget.org/packages?q=mplayercontrol.

* PM> Install-Package MPlayerControl
* PM> Install-Package MPlayerControl-Gtk
* PM> Install-Package MPlayerControl-Winform

# Development
Written in c#.

MPlayerControl is developed with the following work flow:

* Nothing happens for months/years
* Someone needs it to do something it doesn't already do
* That person implements that something and submits a pull request
* Repeat if it doesn't have a feature that you want it to have, add it
    * If it has a bug you need fixed, fix it


## Linux and Mac
Use [Rider](https://www.jetbrains.com/rider/).

## Windows
Use [Visual Studio](https://visualstudio.microsoft.com/vs/) 2019 or newer.

## MPlayer

Majorsilence.Media.Videos requires "mplayer.exe" to exist in the same directory as the exe or the path must be specified.  On linux mplayer may be in the path and work without specifying a path or having mplayer in the same directory.  Download mplayer for windows from http://oss.netfarm.it/mplayer-win32.php. It may require some tweaks to getting working on some systems.

## MPV Support
Initial mpv support has been added.
Majorsilence.Media.Videos needs the libmpv path set and he following environment variable set.

The LC_NUMERIC environment variable must be set to "C" before running an that uses LibMPlayerCommon and libmpv.

```
LC_NUMERIC=C
```

Example
```
LC_NUMERIC=C mono MediaPlayer.exe
```

In mono develop right click the executable __Project -> Options -> Run -> General__ add the environment varilabe here.

```
LC_NUMERIC=C
```

## Web and Worker Projects

Majorsilence.Media.Web is a webapi project that can be used to upload videos to a server.   Majorsilence.Media.WorkerService is a dotnet worker service that detects the uploaded videos and converts them to various formats.

The two project communicate through a simple txt file.   Each upload video is assigned a guid.   The text format for communication is as follows:

* filename: {guid}.txt
* Line 1: {guid}
    * Added by the __web upload project__.
* Line 2: {original video file extension}
    * Added by the __web upload project__.
* Line 3: {MM/dd/yyyy HH:mm:ss}
    * optional timestamp of video converstation start date.
    * Added by the __worker service__.

Example upload file

f95a2020-c31c-4d8d-bb86-82b6edf2b529.txt
```text
f95a2020-c31c-4d8d-bb86-82b6edf2b529
.mkv
```

Example after the worker service started processing the video.

f95a2020-c31c-4d8d-bb86-82b6edf2b529.txt
```text
f95a2020-c31c-4d8d-bb86-82b6edf2b529
.mkv
10/29/2022 20:46:23
```

### Web and Worker Service Docker Compose Example


```yml
version: "3.8"
services:
  workerservice:
    image: "majorsilence/media_workerservice:1.0.0-main"
    environment:
        ApiSettings__UploadFolder: "PLACEHOLDER/web/uploads"
        ApiSettings__ConvertedFolder: "PLACEHOLDER/web/output"
        ApiSettings__MEncoderPath: "/usr/bin/mencoder"
        ApiSettings__FfmpegPath: "/usr/bin/ffmpeg"
    command: sysctl -w fs.inotify.max_user_instances=1024
  workerservice:
    image: "majorsilence/media_web:1.0.0-main"
    ports:
        - "30001:80"
    environment:
        ApiSettings__UploadFolder: "PLACEHOLDER/web/uploads"
        ASPNETCORE_URLS: "https://+:443;http://+:80"
        #ASPNETCORE_Kestrel__Certificates__Default__Password: "PLACEHOLDER"
        #ASPNETCORE_Kestrel__Certificates__Default__Path: "/PLACEHOLDER/certs/PLACEHOLDER.pfx"
    command: sysctl -w fs.inotify.max_user_instances=1024
```


## Ubuntu Setup

Core requirements

```bash
sudo apt-get install mplayer mencoder libmpv1 ffmpeg
```


## Release build

```bash
cd Release-Builds
./build-release.ps1
```

Upload nuget packages to nuget.org.


bump version number.

```
./prepare-files-for-release.sh
```

Commit and push new version number for future developments
