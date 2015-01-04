This is a .NET 4.5 wrapper library for MPlayer.  It has been tested on Windows XP/7/8 and Ubuntu Linux 10.10/11.04/12.04/13.04/14.04 and OpenSuse Linux 10.3.  It should also work on OS X although it has not been tested.  Written in C#.

LibMPlayerCommon requires "mplayer.exe" to exist in the same directory as the dll or the path must be specified.  On linux mplayer may be in the path and work without specifying a path or having mplayer in the same directory.  Download mplayer for windows from http://oss.netfarm.it/mplayer-win32.php. It may require some tweaks to getting working on some systems.

It can play both audio and video files. It includes a sample user interface.

It currently supports play, pause, stop. seek and some other basic functionality. I only add new features as I require them or people send in patches.

See examples on the wiki pages https://github.com/majorsilence/MPlayerControl/wiki.

Commercial support available from http://mplayercontrol.majorsilence.com.

# Downloads
Download using nuget https://www.nuget.org/packages?q=mplayercontrol.

* PM> Install-Package MPlayerControl
* PM> Install-Package MPlayerControl-Gtk
* PM> Install-Package MPlayerControl-Winform

## CI Server Downloads
Each change that is pushed is automatically built by the http://build01.majorsilence.com/ ci server.  The last 100 builds are archived.

# Development
MPlayerControl is developed with the following work flow:

* Nothing happens for weeks
* Someone needs it to do something it doesn't already do
* That person implements that something and submits a pull request
* Repeat if it doesn't have a feature that you want it to have, add it
    * If it has a bug you need fixed, fix it
