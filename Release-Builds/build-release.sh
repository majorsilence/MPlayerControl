#!/usr/bin/env bash
set -e # exit on first error
set -u # exit on using unset variable

xbuild "../MPlayerControl.sln" /toolsversion:4.0 /p:Configuration="Release";Platform="Any CPU"
xbuild "../MPlayerGtkWidget.sln" /toolsversion:4.0 /p:Configuration="Release";Platform="Any CPU"

CURRENTPATH=`pwd`
PACKAGEDIR="MPlayerControl-dot-net-4.5-AnyCPU"

rm -rf ./build-output/MPlayerControl-dot-net-4.5-AnyCPU
rm -rf "./build-output/$PACKAGEDIR.zip"
mkdir -p ./build-output/MPlayerControl-dot-net-4.5-AnyCPU

cp ../MediaPlayer/bin/Release/LibImages.dll "./build-output/$PACKAGEDIR/LibImages.dll"
cp ../MediaPlayer/bin/Release/LibMplayerCommon.dll "./build-output/$PACKAGEDIR/LibMplayerCommon.dll"
cp ../MediaPlayer/bin/Release/MediaPlayer.exe "./build-output/$PACKAGEDIR/MediaPlayer.exe"
cp ../LibMPlayerWinform/bin/Release/LibMPlayerWinform.dll "./build-output/$PACKAGEDIR/LibMPlayerWinform.dll"
cp ../MPlayerGtkWidget/bin/Release/MPlayerGtkWidget.dll "./build-output/$PACKAGEDIR/MPlayerGtkWidget.dll"
cp ../SlideShow/bin/Release/SlideShow.exe "./build-output/$PACKAGEDIR/SlideShow.exe"


cd build-output
zip -r "$PACKAGEDIR.zip" "$PACKAGEDIR"
cd ..

# nuget stuff
rm -rf "./build-output/*.nupkg"

# Begin Core Nuget Package
cd nuget
cd MPlayerControl
rm -rf lib/net45
rm -rf content
mkdir -p lib/net45
mkdir content

cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibImages.dll" lib/net45/LibImages.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMplayerCommon.dll" lib/net45/LibMplayerCommon.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/MediaPlayer.exe" content/MediaPlayer.exe

nuget pack "$CURRENTPATH/nuget/MPlayerControl/MPlayerControl.nuspec" -OutputDirectory "$CURRENTPATH/build-output"

cd "$CURRENTPATH"
# End Core Nuget Package


# Begin winform Nuget Package
cd nuget
cd MPlayerControl-Winform
rm -rf lib/net45
mkdir -p lib/net45

cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMPlayerWinform.dll" lib/net45/LibMPlayerWinform.dll

nuget pack "$CURRENTPATH/nuget/MPlayerControl-Winform/MPlayerControl-Winform.nuspec" -OutputDirectory "$CURRENTPATH/build-output"
cd "$CURRENTPATH"
# End winform Nuget Package


# Begin winform Nuget Package
cd nuget
cd MPlayerControl-Gtk
rm -rf lib/net45
mkdir -p lib/net45

cp "$CURRENTPATH/build-output/$PACKAGEDIR/MPlayerGtkWidget.dll" lib/net45/MPlayerGtkWidget.dll

nuget pack "$CURRENTPATH/nuget/MPlayerControl-Gtk/MPlayerControl-Gtk.nuspec" -OutputDirectory "$CURRENTPATH/build-output"
cd "$CURRENTPATH"
# End winform Nuget Package
