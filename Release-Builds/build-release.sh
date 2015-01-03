xbuild "../MPlayerControl.sln" /toolsversion:4.0 /p:Configuration="Release";Platform="AnyCPU"

CURRENTPATH=`pwd`
PACKAGEDIR="MPlayerControl-dot-net-4.5-AnyCPU"

rm -rf ./build-output/MPlayerControl-dot-net-4.5-AnyCPU
mkdir -p ./build-output/MPlayerControl-dot-net-4.5-AnyCPU

cp ../MediaPlayer/bin/Release/LibImages.dll "./build-output/$PACKAGEDIR/LibImages.dll"
cp ../MediaPlayer/bin/Release/LibMplayerCommon.dll "./build-output/$PACKAGEDIR/LibMplayerCommon.dll"
cp ../MediaPlayer/bin/Release/MediaPlayer.exe "./build-output/$PACKAGEDIR/MediaPlayer.exe"


# nuget stuff
cd nuget
cd MPlayerControl
rm -rf lib/net45
rm -rf content
mkdir lib/net45
mkdir content

cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibImages.dll" lib/net45/LibImages.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/LibMplayerCommon.dll" lib/net45/LibMplayerCommon.dll
cp "$CURRENTPATH/build-output/$PACKAGEDIR/MediaPlayer.exe" content/MediaPlayer.exe

nuget pack "$CURRENTPATH/nuget/MPlayerControl/MPlayerControl.nuspec" -OutputDirectory "$CURRENTPATH/build-output"


cd "$CURRENTPATH"